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
    private static FileSystemWatcher? _fileWatcher;

    /// <summary>
    /// Initializes the ApiErrorCacheService with the specified JSON configuration file.
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file containing the error configurations.</param>
    /// <exception cref="ArgumentException">Thrown when the provided path is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the service is already initialized or when there are issues reading the JSON file.</exception>
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

        _fileWatcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite
        };
        _fileWatcher.Changed += OnChanged;
        _fileWatcher.EnableRaisingEvents = true;

        _initialized = true;
    }

    /// <summary>
    /// Event handler triggered when the configuration file changes.
    /// Attempts to reload the error configurations with a retry mechanism.
    /// </summary>
    private static async void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.Information("File change detected. Reloading Error Configurations.");

        bool success = false;
        int retries = 3;

        while (retries > 0 && !success)
        {
            try
            {
                await Task.Delay(500); // Wait for 500 ms before retrying
                LoadErrorsFromJson(e.FullPath);
                success = true;
            }
            catch (IOException)
            {
                retries--;
                _logger.Warning("Failed to reload configurations. Retries left: {Retries}", retries);

                if (retries == 0)
                {
                    _logger.Error("Exhausted all retries for reloading error configurations.");
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Loads error configurations from the specified JSON file into the cache.
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file.</param>
    /// <exception cref="InvalidOperationException">Thrown when the JSON file cannot be deserialized or the configuration is invalid.</exception>
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

            _logger.Information("Caching API errors from configuration");
            _errorCache.Clear();

            foreach (var error in errorConfiguration.Errors)
            {
                _errorCache[error.Key] = error.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load errors from JSON");
            throw new InvalidOperationException($"Failed to load errors from JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves an ApiError by its key from the cache.
    /// </summary>
    /// <param name="errorKey">The key of the error to retrieve.</param>
    /// <returns>The corresponding ApiError if found, otherwise a default unknown error.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is not initialized.</exception>
    public static ApiError GetError(string errorKey)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("ApiErrorCacheService is not initialized.");
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
