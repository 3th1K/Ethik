using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ethik.Utility.Common.Logging;

public class LogTimer : IDisposable
{
    private readonly ILogger _logger;
    private readonly LogLevel _level;
    private readonly LogMessageBuilder _endMessageBuilder;
    private readonly Stopwatch _stopwatch;

    // Flag to detect redundant calls
    private bool _disposed = false;

    public LogTimer(ILogger logger, LogLevel logLevel, string startMessage, LogMessageBuilder endMessageBuilder)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _level = logLevel;
        _stopwatch = Stopwatch.StartNew();
        _endMessageBuilder = endMessageBuilder;

        // Log start message
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.Log(_level, startMessage);
    }

    // Implement IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
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
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.Log(_level, _endMessageBuilder.BuildLog());
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer if necessary.
            // Set large fields to null.
            // _endMessageBuilder and _logger are managed objects in this case, so not needed to be handled here.

            _disposed = true;
        }
    }
}