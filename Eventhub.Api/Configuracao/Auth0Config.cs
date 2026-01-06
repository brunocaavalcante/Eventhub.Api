using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Eventhub.Api.Configuracao;

public static class Auth0Config
{
    public static IServiceCollection ConfigAuth0(this IServiceCollection services, IConfiguration configuration)
    {
        var domain = configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain não configurado");
        var audience = configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Auth0 Audience não configurado");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{domain}/";
            options.Audience = audience;
            options.RequireHttpsMetadata = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = $"https://{domain}/",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
