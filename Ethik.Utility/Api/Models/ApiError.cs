using System.Text.Json.Serialization;

namespace Ethik.Utility.Api.Models;

public class ApiError
{
    public string ErrorCode { get; set; } = "Unknown";
    public string? ErrorMessage { get; set; }
    public string ErrorDescription { get; set; } = string.Empty;
    public string ErrorSolution { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
    public string? Field { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
    public string? StackTrace { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
    public string? InnerException { get; set; }

}
