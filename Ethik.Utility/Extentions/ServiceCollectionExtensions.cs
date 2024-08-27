using Ethik.Utility.Api.Services;
using Ethik.Utility.Jwt.Helpers;
using Ethik.Utility.Jwt.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ethik.Utility.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddErrorCache(this IServiceCollection services, string jsonFilePath)
    {
        ApiErrorCacheService.Initialize(jsonFilePath);
        return services;
    }

    public static IServiceCollection AddJwtHelper(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        var issuer = jwtSettings.GetValue<string>("Issuer");
        var audience = jwtSettings.GetValue<string>("Audience");

        services.Configure<JwtSettings>(jwtSettings);

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

        services.AddSingleton<JwtTokenService>();
        return services;
    }
}



