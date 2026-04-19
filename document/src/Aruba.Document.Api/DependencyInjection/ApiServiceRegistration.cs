using Aruba.Document.Api.Common.Mapping;
using Aruba.Document.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace Aruba.Document.Api.DependencyInjection;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApi(this IServiceCollection services) =>
        services
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true)
            .AddAutoMapper(cfg => cfg.AddProfile<ApiMappingProfile>());

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
            .AddAuthentication("Bearer")
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

    public static IServiceCollection AddSwaggerGenWithBearerAuth(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Bearer token. Example: **Bearer {token}**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(securityRequirement => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer"), [] }
            });
        });
}