using Ethik.Utility.Api.Models;
using Serilog;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Ethik.Utility.Api.Services;

/// <summary>
/// Service for managing API error configurations.
/// Loads error configurations from a JSON file and caches them for retrieval.
/// Monitors the JSON file for changes and reloads the configuration if the file is updated.
/// </summary>
internal class ApiErrorConfigService
{
    private readonly ILogger _logger;
    private readonly string _jsonFilePath;
    private static ConcurrentDictionary<string, ApiError> _errorCache = new ConcurrentDictionary<string, ApiError>();
    private static bool _initialized = false;
    private readonly FileSystemWatcher _fileWatcher;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorConfigService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    /// <param name="jsonFilePath">The path to the JSON file containing error configurations.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="jsonFilePath"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file is not found at <paramref name="jsonFilePath"/>.</exception>
    public ApiErrorConfigService(ILogger logger, string jsonFilePath)
    {
        _logger = logger.ForContext<ApiErrorConfigService>();
        _jsonFilePath = jsonFilePath ?? throw new ArgumentException("Path cannot be null or empty", nameof(jsonFilePath));

        if (!File.Exists(_jsonFilePath))
        {
            throw new FileNotFoundException("Configuration file not found.", _jsonFilePath);
        }

        string fullPath = Path.GetFullPath(jsonFilePath);
        string directory = Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException("Could not determine the directory of the JSON file.");
        string fileName = Path.GetFileName(fullPath);

        _fileWatcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite
        };
        _fileWatcher.Changed += OnChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Initializes the service by loading error configurations from the JSON file.
    /// Sets the service state to initialized.
    /// </summary>
    public void Initialize()
    {
        LoadErrorsFromJson(_jsonFilePath);
        _initialized = true;
    }

    /// <summary>
    /// Event handler triggered when the configuration file changes.
    /// Attempts to reload the error configurations with a retry mechanism.
    /// Disables the file watcher during the reload process to prevent multiple triggers.
    /// </summary>
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.Information("File change detected.");
        _fileWatcher.EnableRaisingEvents = false;

        bool success = false;
        int retries = 3;

        while (retries > 0 && !success)
        {
            try
            {
                // Wait for 500 ms before retrying
                _ = Task.Run(async () => await Task.Delay(500));

                LoadErrorsFromJson(e.FullPath);
                success = true;
            }
            catch (IOException exception)
            {
                retries--;
                _logger.Warning(exception, "Failed to reload configurations. Retries left: {Retries}", retries);

                if (retries == 0)
                {
                    _logger.Error("Exhausted all retries for reloading error configurations.");
                    throw;
                }
            }
            finally
            {
                _fileWatcher.EnableRaisingEvents = true;
            }
        }
    }

    /// <summary>
    /// Loads error configurations from the specified JSON file into the cache.
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file.</param>
    /// <exception cref="InvalidOperationException">Thrown when the JSON file cannot be deserialized or the configuration is invalid.</exception>
    private void LoadErrorsFromJson(string jsonFilePath)
    {
        _logger.Information("Loading Error Configurations.");
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
    /// Retrieves an <see cref="ApiError"/> by its key from the cache.
    /// </summary>
    /// <param name="errorKey">The key of the error to retrieve.</param>
    /// <returns>The corresponding <see cref="ApiError"/> if found, otherwise a default unknown error.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is not initialized.</exception>
    public static ApiError GetError(string errorKey)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("ApiErrorConfigService is not initialized.");
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

    /// <summary>
    /// Disposes of the <see cref="ApiErrorConfigService"/> and performs any necessary cleanup.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _fileWatcher?.Dispose();
            _disposed = true;
        }
    }
}
