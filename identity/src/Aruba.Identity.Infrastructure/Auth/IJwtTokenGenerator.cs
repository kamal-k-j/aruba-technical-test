using Aruba.Identity.Domain.Models;

namespace Aruba.Identity.Infrastructure.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}