using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Eventhub.Api.Configuracao;

public static class KeycloakConfig
{
    public static IServiceCollection ConfigKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.Authority = configuration["Keycloak:Authority"];
            options.IncludeErrorDetails = bool.Parse(configuration["Keycloak:IncludeErrorDetails"]);

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = bool.Parse(configuration["Keycloak:ValidateAudience"]),
                ValidAudience = configuration["Keycloak:ValidAudience"],
                ValidateIssuerSigningKey = bool.Parse(configuration["Keycloak:ValidateIssuerSigningKey"]),
                ValidateIssuer = bool.Parse(configuration["Keycloak:ValidateIssuer"]),
                ValidIssuer = configuration["Keycloak:ValidIssuer"],
                ValidateLifetime = bool.Parse(configuration["Keycloak:ValidateLifetime"])
            };
        });

        return services;
    }
}
