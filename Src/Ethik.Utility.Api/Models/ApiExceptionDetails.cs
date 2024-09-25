using System.Text.Json.Serialization;

namespace Ethik.Utility.Api.Models;

/// <summary>
/// Represents detailed information about an exception.
/// </summary>
public class ApiExceptionDetails
{
    /// <summary>
    /// Gets or sets the type of the exception.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the exception.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the stack trace of the exception.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the details of the inner exception, if any.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApiExceptionDetails? InnerException { get; set; }
}