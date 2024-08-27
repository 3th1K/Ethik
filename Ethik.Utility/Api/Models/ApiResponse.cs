using System.Reflection;
using System.Text.Json.Serialization;
using Ethik.Utility.Api.Services;

namespace Ethik.Utility.Api.Models;
public enum ApiResponseStatus
{
    Success = 1,
    Failure = 0
}
public class ApiResponse<T>
{

    public ApiResponseStatus Status { get; private set; } = ApiResponseStatus.Failure;
    public string Message { get; private set; } = "No message";
    public int StatusCode { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ApiError>? Errors { get; private set; } = null;

    public static ApiResponse<T> Success(T data, int statusCode = 200, string message = "Request was successful.")
    {
        return new ApiResponse<T>
        {
            Status = ApiResponseStatus.Success,
            Message = message,
            StatusCode = statusCode,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(string message, int statusCode, List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Status = ApiResponseStatus.Failure,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
    }
    public static ApiResponse<T> Failure(string errorKey, string message, int statusCode = 500)
    {
        var apiError = ApiErrorCacheService.GetError(errorKey);
        return new ApiResponse<T>
        {
            Status = ApiResponseStatus.Failure,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<ApiError> { apiError }
        };
    }

}



