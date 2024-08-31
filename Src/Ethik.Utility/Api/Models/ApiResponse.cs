using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ethik.Utility.Api.Services;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Ethik.Utility.Api.Models
{
    /// <summary>
    /// Represents the status of an API response.
    /// </summary>
    public enum ApiResponseStatus
    {
        /// <summary>
        /// Indicates that the API request was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates that the API request failed.
        /// </summary>
        Failure
    }

    /// <summary>
    /// Represents a standard response for API operations.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the response.</typeparam>
    [Serializable]
    public class ApiResponse<T>
    {
        private static readonly ILogger _logger = Log.ForContext<ApiResponse<T>>();

        /// <summary>
        /// Gets the status of the API response.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApiResponseStatus Status { get; private set; } = ApiResponseStatus.Failure;

        /// <summary>
        /// Gets a message providing additional information about the response.
        /// </summary>
        public string Message { get; private set; } = "No message";

        /// <summary>
        /// Gets the HTTP status code associated with the response.
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// Gets the data returned in the response.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; private set; }

        /// <summary>
        /// Gets the list of errors associated with the response, if any.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ApiError>? Errors { get; private set; }

        /// <summary>
        /// Creates a successful API response with the specified data.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <param name="message">An optional message to include in the response.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing a successful response.</returns>
        public static ApiResponse<T> Success(T data, int statusCode = StatusCodes.Status200OK, string message = "Request was successful.")
        {
            var response = new ApiResponse<T>
            {
                Status = ApiResponseStatus.Success,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
            _logger.Debug("Wrote Success Response {Response}", response);
            return response;
        }

        /// <summary>
        /// Creates a failed API response with the specified message and status code.
        /// </summary>
        /// <param name="message">An error message to include in the response.</param>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <param name="errors">An optional list of errors to include in the response.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing a failed response.</returns>
        public static ApiResponse<T> Failure(string message, int statusCode, List<ApiError>? errors = null)
        {
            var response = new ApiResponse<T>
            {
                Status = ApiResponseStatus.Failure,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
            _logger.Debug("Wrote Failure Response {Response}", response);
            return response;
        }

        /// <summary>
        /// Creates a failed API response with the specified error key, message, and status code.
        /// </summary>
        /// <param name="errorKey">The key to retrieve the error details from the error cache.</param>
        /// <param name="message">An optional message to include in the response.</param>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing a failed response with error details.</returns>
        public static ApiResponse<T> Failure(string errorKey, string message, int statusCode = StatusCodes.Status500InternalServerError)
        {
            var apiError = ApiErrorConfigService.GetError(errorKey);
            var response = new ApiResponse<T>
            {
                Status = ApiResponseStatus.Failure,
                Message = message,
                StatusCode = statusCode,
                Errors = new List<ApiError> { apiError }
            };
            _logger.Debug("Wrote Failure Response {Response}", response);
            return response;
        }

        /// <summary>
        /// Returns a JSON string representation of the <see cref="ApiResponse{T}"/> object.
        /// </summary>
        /// <returns>A JSON string representing the <see cref="ApiResponse{T}"/> object.</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
