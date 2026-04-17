using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.Search;

public record SearchUsersQuery : IRequest<List<UserResult>>
{
    public required string Field { get; init; }

    public required string Value { get; init; }
}