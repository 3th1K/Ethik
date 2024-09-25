namespace Ethik.Utility.Security.Jwt.Models;

/// <summary>
/// Represents the details of a generated JWT token.
/// </summary>
public class JwtTokenDetails
{
    /// <summary>
    /// Gets the generated JWT token as a string.
    /// </summary>
    public string Token { get; }

    /// <summary>
    /// Gets the expiration date and time of the token.
    /// This indicates when the token will no longer be valid.
    /// </summary>
    public DateTime Expiration { get; }

    /// <summary>
    /// Gets the type of token, typically "Bearer".
    /// </summary>
    public string TokenType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenDetails"/> class.
    /// </summary>
    /// <param name="token">The generated JWT token as a string.</param>
    /// <param name="expiration">The expiration date and time of the token.</param>
    /// <param name="tokenType">The type of the token (e.g., "Bearer").</param>
    public JwtTokenDetails(string token, DateTime expiration, string tokenType)
    {
        Token = token;
        Expiration = expiration;
        TokenType = tokenType;
    }
}