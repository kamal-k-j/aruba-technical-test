using Aruba.Identity.Application.Auth.Responses;
using MediatR;

namespace Aruba.Identity.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginResult>
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}