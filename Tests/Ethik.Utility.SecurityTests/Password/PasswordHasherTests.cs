using NUnit.Framework;

namespace Ethik.Utility.Security.Password.Tests;

[TestFixture]
public class PasswordHasherTests
{
    [Test]
    public void HashPassword_ShouldHashPasswordSuccessfully()
    {
        // Arrange
        string password = "testPassword";

        // Act
        var hashedPassword = PasswordHasher.HashPassword(password);

        // Assert
        Assert.IsNotNull(hashedPassword);
        Assert.IsTrue(hashedPassword.Contains("."));
    }

    [Test]
    public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
    {
        // Arrange
        string password = "testPassword";

        // Act
        var hashedPassword1 = PasswordHasher.HashPassword(password);
        var hashedPassword2 = PasswordHasher.HashPassword(password);

        // Assert
        Assert.That(hashedPassword2, Is.Not.EqualTo(hashedPassword1));
    }

    [Test]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        string password = "testPassword";
        var hashedPassword = PasswordHasher.HashPassword(password);

        // Act
        bool result = PasswordHasher.VerifyPassword(hashedPassword, password);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        string password = "testPassword";
        string incorrectPassword = "wrongPassword";
        var hashedPassword = PasswordHasher.HashPassword(password);

        // Act
        bool result = PasswordHasher.VerifyPassword(hashedPassword, incorrectPassword);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void VerifyPassword_ShouldReturnFalse_ForIncorrectHashedPasswordFormat()
    {
        // Arrange
        string password = "testPassword";
        string invalidHashedPassword = "invalidFormat";

        // Act
        bool result = PasswordHasher.VerifyPassword(invalidHashedPassword, password);

        // Assert
        Assert.IsFalse(result);
    }
}