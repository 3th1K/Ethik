using Ethik.Utility.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Ethik.Utility.Api.Services
{
    /// <summary>
    /// Handles global exceptions and generates appropriate API responses based on the exception type and environment.
    /// </summary>
    internal class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="env">The hosting environment used to determine if the application is in development mode.</param>
        public GlobalExceptionHandler(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Handles exceptions and generates an API response with error details.
        /// </summary>
        /// <param name="httpContext">The HTTP context for the request.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the exception was handled.</returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            List<ApiError> apiErrors;
            ApiResponse<ApiError> apiResponse;

            switch (exception)
            {
                //case ValidationException validationException:
                //    apiErrors = validationException.Errors.Select(e => new ApiError
                //    {
                //        ErrorCode = e.ErrorCode,
                //        Field = e.PropertyName,
                //        ErrorMessage = e.ErrorMessage,
                //    }).ToList();

                //    apiResponse = ApiResponse<ApiError>.Failure("Validation Error", StatusCodes.Status400BadRequest, apiErrors);
                //    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                //    break;

                default:
                    apiErrors = new List<ApiError>();

                    if (_env.IsDevelopment())
                    {
                        var apiError = new ApiError
                        {
                            ErrorCode = "internal_server_error",
                            ErrorMessage = exception.Message,
                            ExceptionObj = exception
                        };
                        apiErrors.Add(apiError);
                    }
                    else
                    {
                        var apiError = new ApiError
                        {
                            ErrorCode = "internal_server_error",
                            ErrorMessage = "An unexpected error occurred. Please try again later."
                        };
                        apiErrors.Add(apiError);
                    }

                    apiResponse = ApiResponse<ApiError>.Failure("Internal Server Error", StatusCodes.Status500InternalServerError, apiErrors);
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);
            return true;
        }
    }
}