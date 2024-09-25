namespace Ethik.Utility.Security.Jwt.Models;

/// <summary>
/// Configuration settings for generating and validating JWT tokens.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets the secret key used for signing the JWT token.
    /// This key should be a strong, private key stored securely.
    /// </summary>
    public string? SecretKey { get; init; }

    /// <summary>
    /// Gets the issuer (the entity issuing the token).
    /// Typically, this represents the authentication server or domain.
    /// </summary>
    public string? Issuer { get; init; }

    /// <summary>
    /// Gets the audience (the intended recipients of the token).
    /// Typically, this is the target application or service that uses the token for authentication.
    /// </summary>
    public string? Audience { get; init; }

    /// <summary>
    /// Gets the expiry time for the token in minutes.
    /// This value defines how long the generated JWT token will remain valid.
    /// </summary>
    public int ExpiryMinutes { get; init; }
}