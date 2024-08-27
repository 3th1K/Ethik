using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Ethik.Utility.Jwt.Models;

namespace Ethik.Utility.Jwt.Helpers;

public class JwtTokenService
{

    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(string userId, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

        var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        string tkn = new JwtSecurityTokenHandler().WriteToken(token);

        return tkn;
    }
    public JwtTokenResponse GetTokenDetails(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            throw new ArgumentException("Invalid token");
        }

        var expiration = jwtToken.ValidTo;
        var tokenType = "Bearer";

        return new JwtTokenResponse(token, expiration, tokenType);
    }
}