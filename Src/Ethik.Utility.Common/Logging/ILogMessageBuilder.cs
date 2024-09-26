namespace Ethik.Utility.Common.Logging;

public interface ILogMessageBuilder
{
    /// <summary>
    /// Sets the application name for the log message.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithApp(string appName);

    /// <summary>
    /// Sets the caller method name for the log message.
    /// </summary>
    /// <param name="methodName">The name of the method that is calling this logger.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithCallerMethod(string methodName);

    /// <summary>
    /// Sets the caller class name for the log message.
    /// </summary>
    /// <param name="filePath">The file path of the caller class.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithCallerClass(string filePath);

    /// <summary>
    /// Sets both the caller method name and class name for the log message.
    /// </summary>
    /// <param name="methodName">The name of the method that is calling this logger.</param>
    /// <param name="filepath">The file path of the caller class.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithCallerMethodAndClass(string methodName, string filepath);

    /// <summary>
    /// Sets the log message content.
    /// </summary>
    /// <param name="logMessage">The message to log.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithMessage(string logMessage);

    /// <summary>
    /// Sets the elapsed time for the operation being logged.
    /// </summary>
    /// <param name="milliseconds">The time elapsed in milliseconds.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithElapsedTime(long milliseconds);

    /// <summary>
    /// Sets the user performing the action in the log message.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithUser(string username);

    /// <summary>
    /// Sets contextual information for the log message.
    /// </summary>
    /// <param name="logContext">The context of the log entry.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithContext(string logContext);

    /// <summary>
    /// Adds a correlation ID to the contextual information of the log message.
    /// </summary>
    /// <param name="correlationId">The correlation ID for tracking.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithCorrelationId(string correlationId);

    /// <summary>
    /// Sets the log severity level.
    /// </summary>
    /// <param name="logSeverity">The severity of the log (e.g., Info, Warning, Error).</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithSeverity(string logSeverity);

    /// <summary>
    /// Adds a custom property to the log message.
    /// </summary>
    /// <param name="property">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithProperty(string property, string value);

    /// <summary>
    /// Sets exception details to be included in the log message.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithException(Exception ex);

    /// <summary>
    /// Sets the line number where the log entry was created.
    /// </summary>
    /// <param name="line">The line number in the source code.</param>
    /// <returns>The current instance of <see cref="ILogMessageBuilder"/> for chaining.</returns>
    ILogMessageBuilder WithLineNumber(int line);

    /// <summary>
    /// Builds the final log message as a string.
    /// </summary>
    /// <returns>The constructed log message.</returns>
    string BuildLog();

    /// <summary>
    /// Asynchronously builds the final log message as a string.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing the constructed log message.</returns>
    Task<string> BuildLogAsync();

    /// <summary>
    /// Builds the final log message in JSON format.
    /// </summary>
    /// <returns>The constructed log message as a JSON string.</returns>
    string BuildLogJson();
}
