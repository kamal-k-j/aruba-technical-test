using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Users.Commands.Update;

public record UpdateUserCommand : IRequest<UserResult>
{
    public required string Id { get; init; }

    public required string UserName { get; init; }

    public required string Email { get; init; }

    public string Address { get; init; }

    public required string Password { get; init; }
}