using Aruba.Identity.Api.Common.Mapping;
using Aruba.Identity.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aruba.Identity.Api.DependencyInjection;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApi(this IServiceCollection services) =>
        services
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true)
            .AddAutoMapper(cfg => cfg.AddProfile<ApiMappingProfile>());

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                var settings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret))
                };
            });

        return services;
    }
}