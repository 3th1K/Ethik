using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Ethik.Utility.Api.Models
{
    /// <summary>
    /// Represents an API error with detailed information for error handling and reporting.
    /// </summary>
    [Serializable]
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the unique error code.
        /// </summary>
        public string ErrorCode { get; set; } = "Unknown";

        /// <summary>
        /// Gets or sets a descriptive error message.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the error.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the suggested solution for the error.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorSolution { get; set; }

        /// <summary>
        /// Gets or sets the name of the field associated with the error (if applicable).
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Field { get; set; }

        /// <summary>
        /// Gets or sets detailed information about the exception that caused the error.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ApiExceptionDetails? Exception { get; private set; }

        private Exception? _exceptionObj;

        /// <summary>
        /// Gets or sets the exception object, which is used to populate the <see cref="Exception"/> property.
        /// </summary>
        [JsonIgnore]
        public Exception? ExceptionObj
        {
            set
            {
                _exceptionObj = value;
                if (value != null)
                {
                    Exception = ParseExceptionDetails(value.ToString());
                }
            }
            get => _exceptionObj;
        }

        /// <summary>
        /// Parses exception details from the exception string representation.
        /// </summary>
        /// <param name="exceptionString">The string representation of the exception.</param>
        /// <returns>An <see cref="ApiExceptionDetails"/> object containing parsed details.</returns>
        private static ApiExceptionDetails ParseExceptionDetails(string exceptionString)
        {
            var exceptionDetails = new ApiExceptionDetails();

            // Extract exception type and message
            var match = Regex.Match(exceptionString, @"^(?<type>[\w\.]+): (?<message>.+?)(?=\r\n|$)");
            if (match.Success)
            {
                exceptionDetails.Type = match.Groups["type"].Value;
                exceptionDetails.Message = match.Groups["message"].Value;
            }

            // Extract stack trace
            var stackTraceIndex = exceptionString.IndexOf("   at ");
            if (stackTraceIndex != -1)
            {
                exceptionDetails.StackTrace = exceptionString.Substring(stackTraceIndex)
                    .Split(new[] { "\r\n" }, StringSplitOptions.None)
                    .Where(line => line.Trim().StartsWith("at"))
                    .Select(s => s.Trim())
                    .ToArray();
            }

            // Extract inner exception
            var innerExceptionMatch = Regex.Match(exceptionString, @"(?<= ---> )[\s\S]*?(?=\r\n--- End of inner exception stack trace ---)");
            if (innerExceptionMatch.Success)
            {
                exceptionDetails.InnerException = ParseExceptionDetails(innerExceptionMatch.Value);
            }

            return exceptionDetails;
        }

        /// <summary>
        /// Returns a JSON string representation of the <see cref="ApiError"/> object.
        /// </summary>
        /// <returns>A JSON string representing the <see cref="ApiError"/> object.</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
