namespace Aruba.Identity.Application.Auth.Commands.Responses;

public record LoginResult
{
    public required string Token { get; init; }

    public required DateTime ExpiresAt { get; init; }
}