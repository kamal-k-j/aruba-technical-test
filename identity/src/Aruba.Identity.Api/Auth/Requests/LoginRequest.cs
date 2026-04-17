using System.ComponentModel.DataAnnotations;

namespace Aruba.Identity.Api.Auth.Requests;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [Required]
    public string Password { get; init; }
}