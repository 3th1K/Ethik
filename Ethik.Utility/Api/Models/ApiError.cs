using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Ethik.Utility.Api.Models;

[Serializable]
public class ApiError
{
    public string ErrorCode { get; set; } = "Unknown";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorDescription { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorSolution { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionDetails? Exception { get; private set; }
    
    private Exception? _exceptionObj;

    [JsonIgnore]
    public Exception? ExceptionObj
    {
        set
        {
            _exceptionObj = value;
            if(value is not null)
                Exception = ParseExceptionDetails(value.ToString());
        }
        get => _exceptionObj;
    }

    private ExceptionDetails ParseExceptionDetails(string exceptionString)
    {
        var exceptionDetails = new ExceptionDetails();

        // Extract the exception type and message
        var match = Regex.Match(exceptionString, @"^(?<type>[\w\.]+): (?<message>.+?)(?=\r\n|$)");
        if (match.Success)
        {
            exceptionDetails.Type = match.Groups["type"].Value;
            exceptionDetails.Message = match.Groups["message"].Value;
        }

        // Extract the stack trace
        var stackTraceIndex = exceptionString.IndexOf("   at ");
        if (stackTraceIndex != -1)
        {
            exceptionDetails.StackTrace = exceptionString.Substring(stackTraceIndex)
                .Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Where(line => !string.IsNullOrWhiteSpace(line) && line.StartsWith("   at "))
                .Select(s => s.Trim())
                .ToArray();
        }

        // Check for inner exception
        var innerExceptionMatch = Regex.Match(exceptionString, @"(?<= ---> )[\s\S]*?(?=\r\n--- End of inner exception stack trace ---)");
        if (innerExceptionMatch.Success)
        {
            exceptionDetails.InnerException = ParseExceptionDetails(innerExceptionMatch.Value);
        }

        return exceptionDetails;
    }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}

public class ExceptionDetails
{
    public string? Type { get; set; }
    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? StackTrace { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionDetails? InnerException { get; set; }
}
