using Aruba.Identity.Application.Exceptions;
using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User not found for the id specified: {request.Id}.");
        }

        return _mapper.Map<UserResult>(user);
    }
}