namespace Aruba.Identity.Api.Auth.Responses;

public record LoginResponse
{
    public required string Token { get; init; }

    public required DateTime ExpiresAt { get; init; }
}