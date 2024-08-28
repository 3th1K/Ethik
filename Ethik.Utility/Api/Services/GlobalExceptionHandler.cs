using FluentValidation;
using Ethik.Utility.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Ethik.Utility.Api.Services;
internal class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandler(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var apiErrors = new List<ApiError>();
        ApiResponse<ApiError> apiResponse;
      
        if (exception is ValidationException)
        {
            var ex = exception as ValidationException;
            apiErrors = ex?.Errors.Select(e => new ApiError
            {
                ErrorCode = e.ErrorCode,
                Field = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
            }).ToList();

            apiResponse = ApiResponse<ApiError>.Failure("Validation Error", 400, apiErrors);
            httpContext.Response.StatusCode = 400;
        }
        else
        {
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
            apiResponse = ApiResponse<ApiError>.Failure("Internal server error", 500, apiErrors);
            httpContext.Response.StatusCode = 500;
        }

        await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);
        return true;
    }
}
