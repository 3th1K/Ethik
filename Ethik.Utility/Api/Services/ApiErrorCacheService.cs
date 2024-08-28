using System.Collections.Concurrent;
using System.Text.Json;
using Ethik.Utility.Api.Models;
using Serilog;

namespace Ethik.Utility.Api.Services;

internal static class ApiErrorCacheService
{
    private static readonly ILogger _logger = Log.ForContext(typeof(ApiErrorCacheService));
    private static readonly ConcurrentDictionary<string, ApiError> _errorCache = new ConcurrentDictionary<string, ApiError>();
    private static bool _initialized = false;

    public static void Initialize(string jsonFilePath)
    {
        if (_initialized)
        {
            return;
        }

        _logger.Information("Initializing ApiErrorCacheService");
        
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(jsonFilePath));
        }
        string fullPath = Path.GetFullPath(jsonFilePath);
        string directory = Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException("Could not determine the directory of the JSON file.");
        string fileName = Path.GetFileName(fullPath);

        _logger.Information("Loading Error Configurations");
        LoadErrorsFromJson(jsonFilePath);

        var _fileWatcher = new FileSystemWatcher();
        _fileWatcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite
        };
        _fileWatcher.Changed += OnChanged; 
        
        _fileWatcher.EnableRaisingEvents = true;

        _initialized = true;
    }
    private static async void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.Information("File change detected. Reloading Error Configurations.");

        // Implement retry logic
        bool success = false;
        int retries = 3;
        while (retries > 0 && !success)
        {
            try
            {
                await Task.Delay(500); // Wait for 500 ms before retry
                LoadErrorsFromJson(e.FullPath);
                success = true;
            }
            catch (IOException)
            {
                retries--;
                if (retries == 0) throw;
            }
        }
    }


    private static void LoadErrorsFromJson(string jsonFilePath)
    {
        try
        {
            var json = File.ReadAllText(jsonFilePath);
            var errorConfiguration = JsonSerializer.Deserialize<ApiErrorConfiguration>(json);

            if (errorConfiguration == null || errorConfiguration.Errors == null)
            {
                throw new InvalidOperationException("ErrorConfiguration or Errors list is null.");
            }
            _logger.Information("Caching api errors from configuration");
            _errorCache.Clear();
            foreach (var error in errorConfiguration.Errors)
            {
                _errorCache[error.Key] = error.Value;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load errors from JSON: {ex.Message}", ex);
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
