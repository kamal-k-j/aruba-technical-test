namespace Aruba.Identity.Api.Users.Responses;

public record UserResponse
{
    public required string Id { get; init; }

    public required string UserName { get; init; }

    public required string Email { get; init; }

    public string Address { get; init; }

    public required string PasswordHash { get; init; }
}