using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Domain.Models;
using Aruba.Identity.Infrastructure.Users.Repository;
using AutoMapper;
using MediatR;
using System.Linq.Expressions;

namespace Aruba.Identity.Application.Users.Queries.Search;

public class SearchUsersHandler : IRequestHandler<SearchUsersQuery, List<UserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public SearchUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserResult>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var predicate = BuildPredicate(request.Field, request.Value);

        var users = await _userRepository.FindAsync(predicate);

        return _mapper.Map<List<UserResult>>(users);
    }

    private static Expression<Func<User, bool>> BuildPredicate(string field, string value) => field.ToLower() switch
    {
        "email" => _ => _.Email == value,
        "username" => _ => _.UserName == value,
        "address" => _ => _.Address == value,
        "id" => _ => _.Id == value,
        _ => _ => false
    };
}