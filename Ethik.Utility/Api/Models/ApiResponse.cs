using System.Text.Json;
using System.Text.Json.Serialization;
using Ethik.Utility.Api.Services;
using Serilog;

namespace Ethik.Utility.Api.Models;
public enum ApiResponseStatus
{
    Success,
    Failure
}

[Serializable]
public class ApiResponse<T>
{
    private static readonly ILogger _logger = Log.ForContext<ApiResponse<T>>();

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ApiResponseStatus Status { get; private set; } = ApiResponseStatus.Failure;
    public string Message { get; private set; } = "No message";
    public int StatusCode { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ApiError>? Errors { get; private set; }

    public static ApiResponse<T> Success(T data, int statusCode = 200, string message = "Request was successful.")
    {
        var res = new ApiResponse<T>
        {
            Status = ApiResponseStatus.Success,
            Message = message,
            StatusCode = statusCode,
            Data = data
        };
        _logger.Debug("Wrote Success Response {Response}", res);
        return res;
    }

    public static ApiResponse<T> Failure(string message, int statusCode, List<ApiError>? errors = null)
    {
        var res =  new ApiResponse<T>
        {
            Status = ApiResponseStatus.Failure,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
        _logger.Debug("Wrote Failure Response {Response}", res);
        return res;
    }
    public static ApiResponse<T> Failure(string errorKey, string message, int statusCode = 500)
    {
        var apiError = ApiErrorCacheService.GetError(errorKey);
        var res = new ApiResponse<T>
        {
            Status = ApiResponseStatus.Failure,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<ApiError> { apiError }
        };
        _logger.Debug("Wrote Failure Response {Response}", res);
        return res;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

}

