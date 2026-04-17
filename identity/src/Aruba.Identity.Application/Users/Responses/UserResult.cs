namespace Aruba.Identity.Application.Users.Responses;

public record UserResult
{
    public required string Id { get; init; }

    public required string UserName { get; init; }

    public required string Email { get; init; }

    public string Address { get; init; }

    public required string PasswordHash { get; init; }
}