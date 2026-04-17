using Aruba.Identity.Application.Exceptions;
using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetByEmail;

public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByEmailHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new NotFoundException($"User not found for the email specified: {request.Email}.");
        }

        return _mapper.Map<UserResult>(user);
    }
}