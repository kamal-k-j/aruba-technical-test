using Aruba.Identity.Application.Exceptions;
using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Identity.Application.Users.Commands.Update;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User not found for the id specified: {request.Id}.");
        }

        user.UpdateEmail(request.Email);

        user.UpdateAddress(request.Address);

        user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.Password));

        await _userRepository.UpdateAsync(user);

        user = await _userRepository.GetByIdAsync(user.Id);

        return _mapper.Map<UserResult>(user);
    }
}