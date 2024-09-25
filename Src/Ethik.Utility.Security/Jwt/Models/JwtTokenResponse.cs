namespace Ethik.Utility.Security.Jwt.Models;

[Serializable]
public class JwtTokenResponse
{
    public string Token { get; private set; }
    public DateTime Expiration { get; private set; }
    public string TokenType { get; private set; }

    public JwtTokenResponse(string token, DateTime expiration, string tokenType)
    {
        Token = token;
        Expiration = expiration;
        TokenType = tokenType;
    }
}