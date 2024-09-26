using Microsoft.Extensions.Logging;
using Moq;

namespace Ethik.Utility.Common.Logging.Tests;

[TestFixture]
public class LogTimerTests
{
    private Mock<ILogger> _mockLogger;
    private Mock<ILogMessageBuilder> _mockLogMessageBuilder;
    private LogTimer _logTimer;
    private const LogLevel LogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
    private const string StartMessage = "Starting operation";

    [SetUp]
    public void SetUp()
    {
        // Mock ILogger and LogMessageBuilder
        _mockLogger = new Mock<ILogger>();
        _mockLogMessageBuilder = new Mock<ILogMessageBuilder>();

        // Set up IsEnabled to always return true
        _mockLogger.Setup(logger => logger.IsEnabled(LogLevel)).Returns(true);

        // Set up the LogMessageBuilder to return a specific message when building
        _mockLogMessageBuilder.Setup(builder => builder.BuildLog()).Returns("Operation completed in X ms");
    }

    [Test]
    public void LogTimer_StartsTimerAndLogsStartMessage()
    {
        // Arrange & Act
        using (_logTimer = new LogTimer(_mockLogger.Object, LogLevel, StartMessage, _mockLogMessageBuilder.Object))
        {
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Starting operation" && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        }
    }

    [Test]
    public void LogTimer_StopsTimerAndLogsEndMessage_WithElapsedTime()
    {
        // Arrange
        using (_logTimer = new LogTimer(_mockLogger.Object, LogLevel, StartMessage, _mockLogMessageBuilder.Object))
        {
            // Act - Dispose the timer, which should stop and log the end message
        }

        // Assert that elapsed time was added to the builder
        _mockLogMessageBuilder.Verify(builder => builder.WithElapsedTime(It.IsAny<long>()), Times.Once);
        _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Operation completed in X ms" && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void LogTimer_ThrowsArgumentNullException_IfLoggerIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LogTimer(null, LogLevel, StartMessage, _mockLogMessageBuilder.Object));
    }

    [Test]
    public void LogTimer_OnlyLogsWhenLogLevelIsEnabled()
    {
        // Arrange
        _mockLogger.Setup(logger => logger.IsEnabled(LogLevel)).Returns(false);

        // Act
        using (_logTimer = new LogTimer(_mockLogger.Object, LogLevel, StartMessage, _mockLogMessageBuilder.Object))
        {
            // Assert

            _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Operation completed in X ms" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Never);
        }
    }
}