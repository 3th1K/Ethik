using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Serilog;
using System.Security.Cryptography;

namespace Ethik.Utility.Password;

public static class PasswordHasher
{
    private static readonly ILogger _logger = Log.ForContext(typeof(PasswordHasher));

    public static string HashPassword(string password)
    {
        try
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            _logger.Information("Password hashed successfully.");

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while hashing the password.");
            throw;
        }
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        try
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
            {
                _logger.Warning("Invalid hashed password format.");
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var providedHash = KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

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
