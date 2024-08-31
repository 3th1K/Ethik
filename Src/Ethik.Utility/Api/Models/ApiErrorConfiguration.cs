namespace Ethik.Utility.Api.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the configuration for API errors, containing a dictionary of error codes to their respective error details.
    /// </summary>
    internal class ApiErrorConfiguration
    {
        /// <summary>
        /// Gets or sets a dictionary of error codes to their corresponding <see cref="ApiError"/> details.
        /// </summary>
        public Dictionary<string, ApiError> Errors { get; set; } = new Dictionary<string, ApiError>();
    }
}
