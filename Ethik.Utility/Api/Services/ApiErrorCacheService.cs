using System.Collections.Concurrent;
using System.Text.Json;
using Ethik.Utility.Api.Models;

namespace Ethik.Utility.Api.Services;

public static class ApiErrorCacheService
{
    private static readonly ConcurrentDictionary<string, ApiError> _errorCache = new ConcurrentDictionary<string, ApiError>();
    private static bool _initialized = false;

    public static void Initialize(string jsonFilePath)
    {
        if (_initialized)
        {
            return;
        }

        if (string.IsNullOrEmpty(jsonFilePath))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(jsonFilePath));
        }

        try
        {
            var json = File.ReadAllText(jsonFilePath);
            var errorConfiguration = JsonSerializer.Deserialize<ApiErrorConfiguration>(json);

            if (errorConfiguration == null || errorConfiguration.Errors == null)
            {
                throw new InvalidOperationException("ErrorConfiguration or Errors list is null.");
            }

            foreach (var error in errorConfiguration.Errors)
            {
                _errorCache[error.Key] = error.Value;
            }

            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public static ApiError GetError(string errorKey)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("ErrorCacheService is not initialized.");
        }

        return _errorCache.TryGetValue(errorKey, out var apiError)
            ? apiError
            : new ApiError
            {
                ErrorCode = "UNKNOWN",
                ErrorMessage = "Unknown error.",
                ErrorDescription = "No description available.",
                ErrorSolution = "Contact support."
            };
    }
}
