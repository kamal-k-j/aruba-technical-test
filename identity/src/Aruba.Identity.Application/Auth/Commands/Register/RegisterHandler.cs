using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Domain.Models;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Identity.Application.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, UserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RegisterHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        (
            request.UserName,
            request.Email,
            BCrypt.Net.BCrypt.HashPassword(request.Password),
            request.Address
        );

        await _userRepository.InsertAsync(user);

        user = await _userRepository.GetByIdAsync(user.Id);

        return _mapper.Map<UserResult>(user);
    }
}