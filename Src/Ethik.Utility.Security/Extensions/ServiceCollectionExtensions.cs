using Ethik.Utility.Security.Jwt.Models;
using Ethik.Utility.Security.Jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ethik.Utility.Security.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures JWT authentication and adds the JwtTokenService to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddJwtHelper(this IServiceCollection services, IConfiguration configuration)
    {
        // Retrieve JWT settings from configuration.
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        var issuer = jwtSettings.GetValue<string>("Issuer");
        var audience = jwtSettings.GetValue<string>("Audience");

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidConfigurationException("Jwt configuration is not valid");
        }

        // Configure JWT settings.
        services.Configure<JwtSettings>(jwtSettings);

        // Set up JWT authentication.
        var key = Encoding.UTF8.GetBytes(secretKey);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        // Register JwtTokenService as a singleton.
        services.AddSingleton<JwtTokenService>();
        return services;
    }
}