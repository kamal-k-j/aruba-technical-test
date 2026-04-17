namespace Aruba.Identity.Infrastructure.Auth;

public class JwtSettings
{
    public string Secret { get; set; }

    public double ExpiryMinutes { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }
}