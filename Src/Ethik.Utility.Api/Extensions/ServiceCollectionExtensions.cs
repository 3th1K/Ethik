using Ethik.Utility.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Ethik.Utility.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and initializes the ApiErrorConfigService to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <param name="jsonFilePath">The file path to the JSON file used for error caching.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddErrorConfig(this IServiceCollection services, string jsonFilePath)
    {
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ApiErrorConfigService>>();
            return new ApiErrorConfigService(logger, jsonFilePath);
        });

        // Register the hosted service
        services.AddHostedService<ApiErrorConfigServiceInitializer>();
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