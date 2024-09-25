using System.Text;
using System.Text.Json;

namespace Ethik.Utility.Common.Logging;

public class LogMessageBuilder
{
    private string _app = "";
    private string _callerMethod = "";
    private string _callerClass = "";
    private string _callerMethodAndClass = "";
    private string _message = "";
    private string _elapsed = "";
    private string _user = "";        // To log the user performing the action
    private string _context = "";     // To add contextual information about the log
    private string _severity = "";    // To classify the log severity (Info, Warning, Error, etc.)
    private string _property = "";
    private string _lineNumber = "";
    private string _exception = "";

    // Method to set the application name
    public LogMessageBuilder WithApp(string appName)
    {
        _app = $"[{appName}] ";
        return this;
    }

    // Method to set the caller method name
    public LogMessageBuilder WithCallerMethod(string methodName)
    {
        _callerMethod = $"[Method: {methodName}] ";
        return this;
    }

    // Method to set the caller file path
    public LogMessageBuilder WithCallerClass(string filePath)
    {
        _callerClass = $"[Class: {JustClass(filePath)}] ";
        return this;
    }

    public LogMessageBuilder WithCallerMethodAndClass(string methodName, string filepath)
    {
        _callerMethodAndClass = $"({methodName} -> {JustClass(filepath)}) ";
        return this;
    }

    // Method to set the log message
    public LogMessageBuilder WithMessage(string logMessage)
    {
        if (!string.IsNullOrWhiteSpace(logMessage))
        {
            _message = $"{logMessage} ";
        }
        return this;
    }

    // Method to set the elapsed time
    public LogMessageBuilder WithElapsedTime(long milliseconds)
    {
        _elapsed = $"[Elapsed: {milliseconds.ToString()}ms] ";
        return this;
    }

    // Method to set the user performing the action
    public LogMessageBuilder WithUser(string username)
    {
        _user = $"[User: {username}] ";
        return this;
    }

    // Method to set contextual information about the log
    public LogMessageBuilder WithContext(string logContext)
    {
        _context = $"[Context: {logContext}] ";
        return this;
    }

    public LogMessageBuilder WithCorrelationId(string correlationId)
    {
        _context = $"[CorrelationId: {correlationId}] " + _context;
        return this;
    }

    // Method to set the log severity
    public LogMessageBuilder WithSeverity(string logSeverity)
    {
        _severity = logSeverity;
        return this;
    }

    public LogMessageBuilder WithProperty(string property, string value)
    {
        if (!string.IsNullOrWhiteSpace(property) && !string.IsNullOrWhiteSpace(value))
        {
            _property = $"{_property}[{property}: {value}] ";
        }
        return this;
    }

    public LogMessageBuilder WithException(Exception ex)
    {
        if (ex != null)
        {
            _exception = $"[Exception: {ex.Message}, Inner: {ex.InnerException?.Message}, StackTrace: {ex.StackTrace}] ";
        }
        return this;
    }

    public LogMessageBuilder WithLineNumber(int line)
    {
        _lineNumber = $"[At Line: {line}] ";
        return this;
    }

    // Method to build the log message
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

    public async Task<string> BuildLogAsync()
    {
        return await Task.Run(() => BuildLog());
    }

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

    private static string JustClass(string callerFilePath)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath)) return string.Empty;
        var className = Path.GetFileNameWithoutExtension(callerFilePath);
        return className;
    }
}