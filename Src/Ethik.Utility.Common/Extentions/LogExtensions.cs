using Ethik.Utility.Common.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ethik.Utility.Common.Extentions;

public static class LogExtensions
{
    // Application specific code
    private static string APP = "UNK";  // Unknown yet!

    #region Timers

    public static void SetApplicationName(string appName)
    {
        APP = appName;
    }

    public static Stopwatch Start(this ILogger logger, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var watch = Stopwatch.StartNew();

        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage("Execution Start");

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(builder.BuildLog());
        }
        return watch;
    }

    public static Stopwatch Start(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var watch = Stopwatch.StartNew();

        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(builder.BuildLog());
        }
        return watch;
    }

    public static void Stop(this ILogger logger, Stopwatch watch, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        watch.Stop();

        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage("Execution Finished")
            .WithElapsedTime(watch.ElapsedMilliseconds);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(builder.BuildLog());
        }
    }

    public static void Stop(this ILogger logger, Stopwatch watch, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        watch.Stop();

        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message)
            .WithElapsedTime(watch.ElapsedMilliseconds);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(builder.BuildLog());
        }
    }

    public static IDisposable Watch(this ILogger logger, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFilePath = "")
    {
        var startMessage = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage($"Started execution of {callerMethod} in {JustClass(callerFilePath)}")
            .BuildLog();
        var stopMessageBuilder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage($"Completed execution of {callerMethod} in {JustClass(callerFilePath)}");

        return new LogTimer(logger, LogLevel.Debug, startMessage, stopMessageBuilder);
    }

    public static IDisposable Watch(this ILogger logger, string startMessage, string stopMessage, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFilePath = "")
    {
        var startMsg = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(startMessage)
            .BuildLog();
        var stopMsgBuilder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(stopMessage);

        return new LogTimer(logger, LogLevel.Debug, startMsg, stopMsgBuilder);
    }

    public static IDisposable Watch(this ILogger logger, LogLevel logLevel, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFilePath = "")
    {
        var startMessage = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage($"Started execution of {callerMethod} in {JustClass(callerFilePath)}")
            .BuildLog();
        var stopMessageBuilder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage($"Completed execution of {callerMethod} in {JustClass(callerFilePath)}");

        return new LogTimer(logger, logLevel, startMessage, stopMessageBuilder);
    }

    public static IDisposable Watch(this ILogger logger, LogLevel logLevel, string startMessage, string stopMessage, [CallerMemberName] string callerMethod = "", [CallerFilePath] string callerFilePath = "")
    {
        var startMsg = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(startMessage)
            .BuildLog();
        var stopMsgBuilder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(stopMessage);

        return new LogTimer(logger, logLevel, startMsg, stopMsgBuilder);
    }

    #endregion Timers

    #region Normal Overrides

    public static void Critical(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message)
            .WithLineNumber(lineNumber);

        if (logger.IsEnabled(LogLevel.Critical))
            logger.LogCritical(builder.BuildLog());
    }

    public static void Error(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);

        if (logger.IsEnabled(LogLevel.Error))
            logger.LogError(builder.BuildLog());
    }

    public static void Warning(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);

        if (logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(builder.BuildLog());
    }

    public static void Information(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(builder.BuildLog());
    }

    public static void Debug(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(builder.BuildLog());
    }

    public static void Trace(this ILogger logger, string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message);

        if (logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace(builder.BuildLog());
    }

    #endregion Normal Overrides

    //#region Custom Logging (Lists, Properties, etc.)

    public static void Property(this ILogger logger, string propertyName, object propertyValue, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithProperty(propertyName, propertyValue?.ToString() ?? "null");

        // Log at Debug level if enabled
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(builder.BuildLog());
    }

    public static void Property(this ILogger logger, string message, string propertyName, object propertyValue, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithMessage(message)
            .WithProperty(propertyName, propertyValue?.ToString() ?? "null");

        // Log at Debug level if enabled
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug(builder.BuildLog());
    }

    //// Log a list of items
    //public static void List<T>(this ILogger logger, string listName, List<T> list, string code = "LIST", [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "")
    //{
    //    // Log count of list items when at Info level
    //    if (logger.IsEnabled(LogLevel.Information))
    //    {
    //        var listCount = list.Any() ? list.Count : 0;
    //        logger.LogInformation(LOG_LIST_CNT, APP, code, listName, listCount, callerMethod, JustClass(callerFilePath));
    //    }

    //    // Log each item when at Debug level
    //    if (logger.IsEnabled(LogLevel.Debug))
    //    {
    //        var idx = 0;
    //        foreach (var item in list)
    //            logger.LogDebug(LOG_LIST_ITM, APP, code, listName, idx++, item, callerMethod, JustClass(callerFilePath));
    //    }
    //}

    #region Exceptions

    public static void ExceptionCritical(this ILogger logger, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        ExceptionPrivate(logger, LogLevel.Critical, exception, callerFilePath, callerMethod, lineNumber);
    }

    public static void ExceptionError(this ILogger logger, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        ExceptionPrivate(logger, LogLevel.Error, exception, callerFilePath, callerMethod, lineNumber);
    }

    public static void ExceptionWarning(this ILogger logger, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        ExceptionPrivate(logger, LogLevel.Warning, exception, callerFilePath, callerMethod, lineNumber);
    }

    public static void ExceptionInformation(this ILogger logger, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        ExceptionPrivate(logger, LogLevel.Information, exception, callerFilePath, callerMethod, lineNumber);
    }

    public static void ExceptionDebug(this ILogger logger, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        ExceptionPrivate(logger, LogLevel.Debug, exception, callerFilePath, callerMethod, lineNumber);
    }

    private static void ExceptionPrivate(this ILogger logger,
            LogLevel logLevel, Exception exception,
            [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int lineNumber = 0)
    {
        var builder = new LogMessageBuilder()
            .WithApp(APP)
            .WithCallerMethodAndClass(callerMethod, callerFilePath)
            .WithProperty("ExceptionType", exception.GetType().Name)
            .WithException(exception)
            .WithLineNumber(lineNumber);

        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, builder.BuildLog());
        }
    }

    #endregion Exceptions

    #region Private Methods

    private static string JustClass(string callerFilePath)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath)) return string.Empty;
        var className = Path.GetFileNameWithoutExtension(callerFilePath);
        return className;
    }

    #endregion Private Methods
}