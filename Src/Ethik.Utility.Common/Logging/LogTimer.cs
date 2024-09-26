using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ethik.Utility.Common.Logging;

/// <summary>
/// A class for logging the time taken by a particular operation.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IDisposable"/> interface, allowing it to be used in a <c>using</c> statement.
/// The timer starts when an instance is created and logs the elapsed time when disposed.
/// </remarks>
public class LogTimer : IDisposable
{
    private readonly ILogger _logger;
    private readonly LogLevel _level;
    private readonly ILogMessageBuilder _endMessageBuilder;
    private readonly Stopwatch _stopwatch;

    // Flag to detect redundant calls
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogTimer"/> class.
    /// </summary>
    /// <param name="logger">The logger used to log messages.</param>
    /// <param name="logLevel">The log level to be used for logging messages.</param>
    /// <param name="startMessage">The message to log when the timer starts.</param>
    /// <param name="endMessageBuilder">The <see cref="LogMessageBuilder"/> used to construct the end message.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public LogTimer(ILogger logger, LogLevel logLevel, string startMessage, ILogMessageBuilder endMessageBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _level = logLevel;
        _stopwatch = Stopwatch.StartNew();
        _endMessageBuilder = endMessageBuilder;

        // Log start message
        if (_logger.IsEnabled(_level))
            _logger.Log(_level, startMessage);
    }

    /// <summary>
    /// Disposes of the resources used by the <see cref="LogTimer"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of the dispose pattern.
    /// </summary>
    /// <param name="disposing">True if called from <see cref="Dispose()"/>; false if called from the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                _stopwatch.Stop();
                var elapsedMs = _stopwatch.ElapsedMilliseconds;
                _endMessageBuilder.WithElapsedTime(elapsedMs);

                // Log the end message with elapsed time
                if (_logger.IsEnabled(_level))
                    _logger.Log(_level, _endMessageBuilder.BuildLog());
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer if necessary.
            // Set large fields to null.
            // _endMessageBuilder and _logger are managed objects in this case, so not needed to be handled here.

            _disposed = true;
        }
    }
}