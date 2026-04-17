using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<UserResult>
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    public string Address { get; init; }

    public required string Password { get; init; }
}