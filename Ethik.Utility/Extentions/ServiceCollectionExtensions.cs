using Ethik.Utility.Api.Services;
using Ethik.Utility.Jwt.Helpers;
using Ethik.Utility.Jwt.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Ethik.Utility.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and initializes the ApiErrorCacheService to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <param name="jsonFilePath">The file path to the JSON file used for error caching.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddErrorCache(this IServiceCollection services, string jsonFilePath)
    {
        ApiErrorCacheService.Initialize(jsonFilePath);
        return services;
    }

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

    /// <summary>
    /// Adds global exception handling and ProblemDetails services to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        // Add a global exception handler service.
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Add ProblemDetails for standardized error responses.
        services.AddProblemDetails();
        return services;
    }

    /// <summary>
    /// Adds Swagger with jwt bearer token authentication options enabled
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
        });

        return services;
    }
}
