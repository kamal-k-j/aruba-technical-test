using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetByEmail;

public record GetUserByEmailQuery : IRequest<UserResult>
{
    public required string Email { get; init; }
}