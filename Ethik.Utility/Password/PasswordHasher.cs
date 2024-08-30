using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Serilog;
using System.Security.Cryptography;

namespace Ethik.Utility.Password;

/// <summary>
/// Provides functionality for hashing and verifying passwords.
/// </summary>
public static class PasswordHasher
{
    private static readonly ILogger _logger = Log.ForContext(typeof(PasswordHasher));

    /// <summary>
    /// Hashes a password using the PBKDF2 algorithm with a randomly generated salt.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>A string containing the salt and hashed password, separated by a dot.</returns>
    /// <exception cref="Exception">Throws an exception if hashing fails.</exception>
    public static string HashPassword(string password)
    {
        try
        {
            // Generate a 128-bit salt using a secure PRNG.
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a 256-bit subkey (hash) from the password and salt using PBKDF2.
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            _logger.Information("Password hashed successfully.");

            // Return the salt and hashed password, concatenated with a dot separator.
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while hashing the password.");
            throw;
        }
    }

    /// <summary>
    /// Verifies that a provided password matches a previously hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password with its salt, formatted as "salt.hash".</param>
    /// <param name="providedPassword">The plaintext password to verify.</param>
    /// <returns>True if the provided password matches the hashed password; otherwise, false.</returns>
    /// <exception cref="Exception">Throws an exception if verification fails.</exception>
    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        try
        {
            // Split the hashed password into its salt and hash components.
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
            {
                _logger.Warning("Invalid hashed password format.");
                return false;
            }

            // Decode the salt and hash from Base64.
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            // Hash the provided password with the same salt.
            var providedHash = KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            // Compare the hash of the provided password with the stored hash.
            bool isMatch = hash.SequenceEqual(providedHash);

            if (isMatch)
            {
                _logger.Information("Password verified successfully.");
            }
            else
            {
                _logger.Warning("Password verification failed.");
            }

            return isMatch;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while verifying the password.");
            throw;
        }
    }
}
