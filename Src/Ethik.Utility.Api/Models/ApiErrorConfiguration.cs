using System.Collections.Concurrent;

namespace Ethik.Utility.Api.Models;

/// <summary>
/// Represents the configuration for API errors, containing a dictionary of error codes to their respective error details.
/// </summary>
public class ApiErrorConfiguration
{
    /// <summary>
    /// Gets or sets a dictionary of error codes to their corresponding <see cref="ApiError"/> details.
    /// </summary>
    public ConcurrentDictionary<string, ApiError> Errors { get; set; } = new ConcurrentDictionary<string, ApiError>();
}