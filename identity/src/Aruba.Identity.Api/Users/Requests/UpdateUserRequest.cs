using System.ComponentModel.DataAnnotations;

namespace Aruba.Identity.Api.Users.Requests;

public record UpdateUserRequest
{
    [Required]
    [MaxLength(20)]
    public string UserName { get; init; }

    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [MaxLength(50)]
    public string Address { get; init; }

    [Required]
    public string Password { get; init; }
}