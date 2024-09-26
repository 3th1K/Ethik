using NUnit.Framework;
using Ethik.Utility.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ethik.Utility.Api.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Moq;
using System.Text.Json;

namespace Ethik.Utility.Api.Services.Tests;

[TestFixture]
public class ApiErrorConfigServiceTests
{
    private Mock<ILogger<ApiErrorConfigService>> _mockLogger;
    private string _testJsonFilePath;
    private ApiErrorConfigService _service;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<ApiErrorConfigService>>();
        _testJsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-errors.json");

        // Write a valid error config JSON file for testing.
        var errorConfig = new ApiErrorConfiguration
        {
            Errors = new ConcurrentDictionary<string, ApiError>
            {
                ["ERR001"] = new ApiError { ErrorCode = "ERR001", ErrorMessage = "Test Error", ErrorDescription = "Test Description", ErrorSolution = "Test Solution" }
            }
        };
        File.WriteAllText(_testJsonFilePath, JsonSerializer.Serialize(errorConfig));
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testJsonFilePath))
        {
            File.Delete(_testJsonFilePath);
        }
        _service.DisposeObject();
    }

    [Test, Order(1)]
    public void GetError_UninitializedService_ThrowsInvalidOperationException()
    {
        // Arrange
        _service = new ApiErrorConfigService(_mockLogger.Object, _testJsonFilePath);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ApiErrorConfigService.GetError("ERR001"));
    }

    [Test, Order(2)]
    public void Initialize_InvalidJsonFile_ThrowsException()
    {
        // Arrange
        var invalidJsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invalid-errors.json");
        File.WriteAllText(invalidJsonFilePath, "Invalid JSON content");
        _service = new ApiErrorConfigService(_mockLogger.Object, invalidJsonFilePath);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.Initialize());

        // Clean up
        if (File.Exists(invalidJsonFilePath))
        {
            File.Delete(invalidJsonFilePath);
        }
    }

    [Test]
    public void Initialize_ValidJsonFile_LoadsErrorsAndInitializes()
    {
        // Arrange
        _service = new ApiErrorConfigService(_mockLogger.Object, _testJsonFilePath);

        // Act
        _service.Initialize();

        // Assert
        var error = ApiErrorConfigService.GetError("ERR001");
        Assert.That(error.ErrorCode, Is.EqualTo("ERR001"));
        Assert.That(error.ErrorMessage, Is.EqualTo("Test Error"));
    }

    [Test]
    public void GetError_ReturnsUnknownError_WhenKeyNotFound()
    {
        // Arrange
        _service = new ApiErrorConfigService(_mockLogger.Object, _testJsonFilePath);
        _service.Initialize();

        // Act
        var error = ApiErrorConfigService.GetError("INVALID_KEY");

        // Assert
        Assert.That(error.ErrorCode, Is.EqualTo("UNKNOWN"));
        Assert.That(error.ErrorMessage, Is.EqualTo("Unknown error."));
    }

    [Test]
    public async Task OnChanged_FileChanged_ReloadsErrors()
    {
        // Arrange
        _service = new ApiErrorConfigService(_mockLogger.Object, _testJsonFilePath);
        _service.Initialize();

        var newErrorConfig = new ApiErrorConfiguration
        {
            Errors = new ConcurrentDictionary<string, ApiError>
            {
                ["ERR002"] = new ApiError { ErrorCode = "ERR002", ErrorMessage = "New Error", ErrorDescription = "New Description", ErrorSolution = "New Solution" }
            }
        };// Act
          // Modify the JSON file to simulate a change
        File.WriteAllText(_testJsonFilePath, JsonSerializer.Serialize(newErrorConfig));

        // Wait for a short period to allow the FileSystemWatcher to trigger the OnChanged event
        await Task.Delay(1000); // Delay for 1 second to let the event handler execute

        // Assert
        var error = ApiErrorConfigService.GetError("ERR002");
        Assert.AreEqual("ERR002", error.ErrorCode);
        Assert.AreEqual("New Error", error.ErrorMessage);
    }

    [Test]
    public void DisposeObject_DisposesFileWatcher()
    {
        // Arrange
        _service = new ApiErrorConfigService(_mockLogger.Object, _testJsonFilePath);
        var fileWatcherField = typeof(ApiErrorConfigService).GetField("_fileWatcher", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fileWatcher = (FileSystemWatcher)fileWatcherField.GetValue(_service);

        // Act
        _service.DisposeObject();

        // Assert
        Assert.IsTrue(_service.GetType().GetField("_disposed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_service) as bool?);
        Assert.Throws<ObjectDisposedException>(() => fileWatcher.EnableRaisingEvents = true);
    }
}