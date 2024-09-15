using NUnit.Framework;
using Microsoft.Extensions.Options;
using Ethik.Utility.Jwt.Models;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Assert = NUnit.Framework.Assert;
using Microsoft.IdentityModel.Tokens;

namespace Ethik.Utility.Jwt.Helpers.Tests
{
    [TestFixture]
    public class JwtTokenServiceTests
    {
        private Mock<IOptions<JwtSettings>> _mockJwtSettings = null!;
        private JwtSettings _jwtSettings = null!;
        private JwtTokenService _sut = null!;

        [SetUp]
        public void Setup() 
        {
            _jwtSettings = new JwtSettings
            {
                Issuer = "testIssuer",
                Audience = "testAudience",
                ExpiryMinutes = 60,
                SecretKey = "SuperSecretKey12345678909876543211234567890"
            };

            _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            _mockJwtSettings.Setup(s => s.Value).Returns(_jwtSettings);

            _sut = new JwtTokenService(_mockJwtSettings.Object);
        }

        [Test]
        public void GenerateToken_ShouldReturnToken_WithCorrectClaims()
        {
            // Arrange
            var userId = "12345";
            var email = "test@example.com";
            var role = "Admin";

            // Act
            var token = _sut.GenerateToken(userId, email, role);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.AreEqual(_jwtSettings.Issuer, jwtToken.Issuer);
            Assert.AreEqual(_jwtSettings.Audience, jwtToken.Audiences.First());
            Assert.AreEqual(userId, jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.AreEqual(email, jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value);
            Assert.AreEqual(role, jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value);
        }

        [Test]
        public void GetTokenDetails_ShouldReturnCorrectDetails_ForValidToken()
        {
            // Arrange
            var userId = "12345";
            var email = "test@example.com";
            var role = "Admin";
            var token = _sut.GenerateToken(userId, email, role);

            // Act
            var result = _sut.GetTokenDetails(token);

            // Assert
            Assert.AreEqual(token, result.Token);
            Assert.AreEqual("Bearer", result.TokenType);
            Assert.IsTrue(result.Expiration > DateTime.UtcNow);
        }

        [Test]
        public void GetTokenDetails_SecurityTokenMalformedException_ForInvalidToken()
        {
            // Arrange
            var invalidToken = "invalidTokenString";

            // Act & Assert
            Assert.Throws<SecurityTokenMalformedException>(() => _sut.GetTokenDetails(invalidToken));
        }
    }
}