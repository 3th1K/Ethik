using NUnit.Framework;
using Ethik.Utility.Security.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ethik.Utility.Security.Jwt.Models;
using Ethik.Utility.Security.Jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using Moq;

namespace Ethik.Utility.Security.Extensions.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IConfigurationSection> _mockJwtSection;
    private IServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        // Set up mocks for configuration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockJwtSection = new Mock<IConfigurationSection>();

        _mockConfiguration
            .Setup(c => c.GetSection("JwtSettings"))
            .Returns(_mockJwtSection.Object);

        _mockJwtSection
            .Setup(s => s.GetValue<string>("SecretKey"))
            .Returns("MySuperSecretKey");
        _mockJwtSection
            .Setup(s => s.GetValue<string>("Issuer"))
            .Returns("MyIssuer");
        _mockJwtSection
            .Setup(s => s.GetValue<string>("Audience"))
            .Returns("MyAudience");

        // Initialize the service collection
        _services = new ServiceCollection();
    }

    [Test]
    public void AddJwtHelper_ShouldRegisterJwtSettings()
    {
        // Act
        _services.AddJwtHelper(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert JwtSettings are configured
        var jwtSettings = serviceProvider.GetService<IOptions<JwtSettings>>();
        Assert.IsNotNull(jwtSettings, "JwtSettings should be registered");
        Assert.AreEqual("MyIssuer", jwtSettings.Value.Issuer);
        Assert.AreEqual("MyAudience", jwtSettings.Value.Audience);
    }

    [Test]
    public void AddJwtHelper_ShouldRegisterJwtTokenService()
    {
        // Act
        _services.AddJwtHelper(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert JwtTokenService is registered
        var jwtTokenService = serviceProvider.GetService<JwtTokenService>();
        Assert.IsNotNull(jwtTokenService, "JwtTokenService should be registered as a singleton");
    }

    [Test]
    public void AddJwtHelper_ShouldRegisterJwtBearerAuthentication()
    {
        // Act
        _services.AddJwtHelper(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Check that authentication has been added
        var authenticationSchemeProvider = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        Assert.IsNotNull(authenticationSchemeProvider, "Authentication should be registered");

        // Verify that JwtBearer is the default authentication scheme
        var schemes = authenticationSchemeProvider.GetAllSchemesAsync().Result;
        Assert.IsTrue(schemes.Any(s => s.Name == JwtBearerDefaults.AuthenticationScheme), "JwtBearer authentication should be registered");
    }

    [Test]
    public void AddJwtHelper_InvalidConfiguration_ShouldThrowException()
    {
        // Arrange: Setup invalid configuration (null SecretKey)
        _mockJwtSection
            .Setup(s => s.GetValue<string>("SecretKey"))
            .Returns((string)null);

        // Act & Assert
        var ex = Assert.Throws<InvalidConfigurationException>(() => _services.AddJwtHelper(_mockConfiguration.Object));
        Assert.AreEqual("Jwt configuration is not valid", ex.Message);
    }
}