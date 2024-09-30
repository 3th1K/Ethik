
# Ethik.Utility.Common NuGet Package

## Overview
The `Ethik.Utility.Common` NuGet package provides a set of utilities for handling Common tasks in .NET applications.

## Installation
You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```bash
Install-Package Ethik.Utility.Common
```

Or, add the package to your .csproj file:

```bash
<PackageReference Include="Ethik.Utility.Common" Version="1.0.0" />
```


## Components

### `LogExtensions` Class
Provides extension methods for `ILogger`, enabling advanced logging capabilities with execution timing and application context.

#### Methods

- **`SetApplicationName(string appName)`**:
    - **Description**: Sets the application name used in log messages.
    - **Parameters**:
        - `appName`: The name of the application to set.
    - **Example Usage**:
      ```csharp
      LogExtensions.SetApplicationName("MyApp");
      ```

### Execution Timing Methods

- **`Start(this ILogger logger, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Starts a stopwatch to measure execution time and logs the start message.
    - **Parameters**:
        - `logger`: The logger instance.
    - **Returns**: A `Stopwatch` instance to measure elapsed time.
    - **Example Usage**:
      ```csharp
      var watch = logger.Start();
      // Method execution...
      logger.Stop(watch);
      ```

- **`Stop(this ILogger logger, Stopwatch watch, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Stops the stopwatch and logs the elapsed time.
    - **Parameters**:
        - `logger`: The logger instance.
        - `watch`: The `Stopwatch` instance to stop.
    - **Example Usage**:
      ```csharp
      logger.Stop(watch);
      ```

- **`Watch(this ILogger logger, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFilePath = "")`**:
    - **Description**: Returns an `IDisposable` that automatically logs the start and stop messages when disposed.
    - **Parameters**:
        - `logger`: The logger instance.
    - **Returns**: An `IDisposable` for disposing.
    - **Example Usage**:
      ```csharp
      using (logger.Watch())
      {
          // Method execution...
      }
      ```

---

### Logging Override Methods

- **`Critical(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)`**:
    - **Description**: Logs a critical message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Critical("A critical error occurred.");
      ```

- **`Error(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Logs an error message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Error("An error occurred while processing.");
      ```

- **`Warning(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Logs a warning message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Warning("This is a warning message.");
      ```

- **`Information(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Logs an informational message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Information("This is an informational message.");
      ```

- **`Debug(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Logs a debug message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Debug("Debugging information.");
      ```

- **`Trace(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")`**:
    - **Description**: Logs a trace message with application and caller information.
    - **Parameters**:
        - `logger`: The logger instance.
        - `message`: The message to log.
    - **Example Usage**:
      ```csharp
      logger.Trace("Tracing execution.");
      ```

---

### `TaskExtensions` Class
Provides extension methods for working with tasks.

#### Methods

- **`WhenAnyNotNullAsync<T>(this IEnumerable<Task<T?>> tasks, CancellationToken cancellationToken)`**:
    - **Description**: Returns the first successfully completed task that is not null from a collection of tasks, or null if no such task exists.
    - **Type Parameters**:
        - `T`: The type of the task result.
    - **Parameters**:
        - `tasks`: A collection of tasks to monitor.
        - `cancellationToken`: A cancellation token to signal cancellation.
    - **Returns**: A task that represents the first successfully completed task with a non-null result. If all tasks complete without a non-null result, returns null.
    - **Example Usage**:
      ```csharp
      var result = await taskCollection.WhenAnyNotNullAsync(cancellationToken);
      ```

---

### `LogMessageBuilder` Class
A builder class for constructing log messages with various contextual information.

#### Remarks
This class allows for flexible construction of log messages by chaining method calls. It supports building log messages in both plain text and JSON formats.

#### Methods

- **`WithApp(string appName)`**:
    - **Description**: Sets the application name for the log message.
    - **Parameters**:
        - `appName`: The name of the application.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithApp("MyApp");
      ```

- **`WithCallerMethod(string methodName)`**:
    - **Description**: Sets the caller method name for the log message.
    - **Parameters**:
        - `methodName`: The name of the method that is calling this logger.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithCallerMethod("Execute");
      ```

- **`WithCallerClass(string filePath)`**:
    - **Description**: Sets the caller class name for the log message.
    - **Parameters**:
        - `filePath`: The file path of the caller class.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithCallerClass("MyService.cs");
      ```

- **`WithCallerMethodAndClass(string methodName, string filePath)`**:
    - **Description**: Sets both the caller method name and class name for the log message.
    - **Parameters**:
        - `methodName`: The name of the method that is calling this logger.
        - `filePath`: The file path of the caller class.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithCallerMethodAndClass("Execute", "MyService.cs");
      ```

- **`WithMessage(string logMessage)`**:
    - **Description**: Sets the log message content.
    - **Parameters**:
        - `logMessage`: The message to log.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithMessage("Operation completed successfully.");
      ```

- **`WithElapsedTime(long milliseconds)`**:
    - **Description**: Sets the elapsed time for the operation being logged.
    - **Parameters**:
        - `milliseconds`: The time elapsed in milliseconds.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithElapsedTime(123);
      ```

- **`WithUser(string username)`**:
    - **Description**: Sets the user performing the action in the log message.
    - **Parameters**:
        - `username`: The username of the user.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithUser("JohnDoe

");
      ```

- **`WithContext(string logContext)`**:
    - **Description**: Sets the context for the log message.
    - **Parameters**:
        - `logContext`: The context in which the logging is done.
    - **Returns**: The current instance of `ILogMessageBuilder` for chaining.
    - **Example Usage**:
      ```csharp
      var logMessage = new LogMessageBuilder()
          .WithContext("Processing order");
      ```

- **`ToJson()`**:
    - **Description**: Converts the constructed log message to a JSON string format.
    - **Returns**: A JSON string representation of the log message.
    - **Example Usage**:
      ```csharp
      string jsonMessage = logMessage.ToJson();
      ```

- **`ToString()`**:
    - **Description**: Converts the constructed log message to a plain text string format.
    - **Returns**: A string representation of the log message.
    - **Example Usage**:
      ```csharp
      string textMessage = logMessage.ToString();
      ```