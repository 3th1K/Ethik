using System.Text;
using System.Text.Json;

namespace Ethik.Utility.Common.Logging;

/// <summary>
/// A builder class for constructing log messages with various contextual information.
/// </summary>
/// <remarks>
/// This class allows for flexible construction of log messages by chaining method calls.
/// It supports building log messages in both plain text and JSON formats.
/// </remarks>
public class LogMessageBuilder : ILogMessageBuilder
{
    private string _app = "";
    private string _callerMethod = "";
    private string _callerClass = "";
    private string _callerMethodAndClass = "";
    private string _message = "";
    private string _elapsed = "";
    private string _user = "";
    private string _context = "";
    private string _severity = "";
    private string _property = "";
    private string _lineNumber = "";
    private string _exception = "";

    /// <summary>
    /// Sets the application name for the log message.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithApp(string appName)
    {
        _app = $"[{appName}] ";
        return this;
    }

    /// <summary>
    /// Sets the caller method name for the log message.
    /// </summary>
    /// <param name="methodName">The name of the method that is calling this logger.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithCallerMethod(string methodName)
    {
        _callerMethod = $"[Method: {methodName}] ";
        return this;
    }

    /// <summary>
    /// Sets the caller class name for the log message.
    /// </summary>
    /// <param name="filePath">The file path of the caller class.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithCallerClass(string filePath)
    {
        _callerClass = $"[Class: {JustClass(filePath)}] ";
        return this;
    }

    /// <summary>
    /// Sets both the caller method name and class name for the log message.
    /// </summary>
    /// <param name="methodName">The name of the method that is calling this logger.</param>
    /// <param name="filepath">The file path of the caller class.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithCallerMethodAndClass(string methodName, string filepath)
    {
        _callerMethodAndClass = $"({methodName} -> {JustClass(filepath)}) ";
        return this;
    }

    /// <summary>
    /// Sets the log message content.
    /// </summary>
    /// <param name="logMessage">The message to log.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithMessage(string logMessage)
    {
        if (!string.IsNullOrWhiteSpace(logMessage))
        {
            _message = $"{logMessage} ";
        }
        return this;
    }

    /// <summary>
    /// Sets the elapsed time for the operation being logged.
    /// </summary>
    /// <param name="milliseconds">The time elapsed in milliseconds.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithElapsedTime(long milliseconds)
    {
        _elapsed = $"[Elapsed: {milliseconds.ToString()}ms] ";
        return this;
    }

    /// <summary>
    /// Sets the user performing the action in the log message.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithUser(string username)
    {
        _user = $"[User: {username}] ";
        return this;
    }

    /// <summary>
    /// Sets contextual information for the log message.
    /// </summary>
    /// <param name="logContext">The context of the log entry.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithContext(string logContext)
    {
        _context = $"[Context: {logContext}] ";
        return this;
    }

    /// <summary>
    /// Adds a correlation ID to the contextual information of the log message.
    /// </summary>
    /// <param name="correlationId">The correlation ID for tracking.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithCorrelationId(string correlationId)
    {
        _context = $"[CorrelationId: {correlationId}] " + _context;
        return this;
    }

    /// <summary>
    /// Sets the log severity level.
    /// </summary>
    /// <param name="logSeverity">The severity of the log (e.g., Info, Warning, Error).</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithSeverity(string logSeverity)
    {
        _severity = logSeverity;
        return this;
    }

    /// <summary>
    /// Adds a custom property to the log message.
    /// </summary>
    /// <param name="property">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithProperty(string property, string value)
    {
        if (!string.IsNullOrWhiteSpace(property) && !string.IsNullOrWhiteSpace(value))
        {
            _property = $"{_property}[{property}: {value}] ";
        }
        return this;
    }

    /// <summary>
    /// Sets exception details to be included in the log message.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithException(Exception ex)
    {
        if (ex != null)
        {
            _exception = $"[Exception: {ex.Message}, Inner: {ex.InnerException?.Message}, StackTrace: {ex.StackTrace}] ";
        }
        return this;
    }

    /// <summary>
    /// Sets the line number where the log entry was created.
    /// </summary>
    /// <param name="line">The line number in the source code.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    public ILogMessageBuilder WithLineNumber(int line)
    {
        _lineNumber = $"[At Line: {line}] ";
        return this;
    }

    /// <summary>
    /// Builds the final log message as a string.
    /// </summary>
    /// <returns>The constructed log message.</returns>
    public string BuildLog()
    {
        var logBuilder = new StringBuilder();
        logBuilder.Append(_app);
        logBuilder.Append(_severity);
        logBuilder.Append(_callerMethod);
        logBuilder.Append(_callerClass);
        logBuilder.Append(_callerMethodAndClass);
        logBuilder.Append(_message);
        logBuilder.Append(_property);
        logBuilder.Append(_user);
        logBuilder.Append(_context);
        logBuilder.Append(_elapsed);
        logBuilder.Append(_lineNumber);
        logBuilder.Append(_exception);

        return logBuilder.ToString().TrimEnd();
    }

    /// <summary>
    /// Asynchronously builds the final log message as a string.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing the constructed log message.</returns>
    public async Task<string> BuildLogAsync()
    {
        return await Task.Run(() => BuildLog());
    }

    /// <summary>
    /// Builds the final log message in JSON format.
    /// </summary>
    /// <returns>The constructed log message as a JSON string.</returns>
    public string BuildLogJson()
    {
        var logObject = new
        {
            App = _app,
            Severity = _severity,
            Method = _callerMethod,
            Class = _callerClass,
            Message = _message,
            User = _user,
            Context = _context,
            Elapsed = _elapsed,
            LineNumber = _lineNumber,
            Exception = _exception
        };
        return JsonSerializer.Serialize(logObject);
    }

    /// <summary>
    /// Extracts just the class name from the full file path.
    /// </summary>
    /// <param name="callerFilePath">The full file path of the caller class.</param>
    /// <returns>The name of the class without the file extension.</returns>
    private static string JustClass(string callerFilePath)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath)) return string.Empty;
        var className = Path.GetFileNameWithoutExtension(callerFilePath);
        return className;
    }
}