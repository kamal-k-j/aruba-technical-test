using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetAll;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<UserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserResult>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();

        return _mapper.Map<List<UserResult>>(users);
    }
}