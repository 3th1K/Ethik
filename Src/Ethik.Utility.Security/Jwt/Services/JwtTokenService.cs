using Ethik.Utility.Security.Jwt.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ethik.Utility.Security.Jwt.Services;

/// <summary>
/// Provides functionality for generating and validating JWT tokens.
/// </summary>
public class JwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="jwtSettings">The JWT settings configured for token generation.</param>
    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings), "Jwt settings config is invalid/null");
        if (_jwtSettings.Issuer is null || _jwtSettings.SecretKey is null || _jwtSettings.Audience is null)
        {
            throw new ArgumentNullException(nameof(jwtSettings), "Jwt settings config is invalid/null");
        }
    }

    /// <summary>
    /// Generates a JWT token based on the provided user details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="role">The role of the user (e.g., Admin, User).</param>
    /// <returns>A string representation of the generated JWT token.</returns>
    public string GenerateToken(string userId, string email, string role)
    {
        // Create security key and credentials using the secret key from settings.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Define claims to include in the token (e.g., user ID, email, role).
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        // Generate the JWT token with the specified claims, issuer, audience, and expiration time.
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        // Return the serialized token as a string.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Retrieves details about the provided JWT token.
    /// </summary>
    /// <param name="token">The JWT token to extract details from.</param>
    /// <returns>A <see cref="JwtTokenResponse"/> object containing token details such as expiration and type.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided token is invalid.</exception>
    public JwtTokenResponse GetTokenDetails(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            throw new ArgumentException("Invalid token");
        }

        // Extract expiration time and set token type as "Bearer".
        var expiration = jwtToken.ValidTo;
        var tokenType = "Bearer";

        // Return the details of the token.
        return new JwtTokenResponse(token, expiration, tokenType);
    }
}