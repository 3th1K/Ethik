# Ethik.Utility.Api NuGet Package

## Overview

The `Ethik.Utility.Api` NuGet package provides a set of utilities for handling API tasks in .NET applications. It includes error handling, response generation, and useful extensions

## **Features**
- **`API Responses`**: Consistent API response format for success and failure scenarios.
- **`Service Collection Extensions`**:
  - **`Error Caching`**:
    - **`AddErrorConfig`**: Adds and initializes `ApiErrorConfigService` to manage error responses with configuration from a JSON file.
  - **`Global Exception Handling`**:
    - **`AddGlobalExceptionHandler`**: Adds global exception handling and `ProblemDetails` services for standardized error responses.
  - **`Swagger with JWT`**:
    - **`AddSwaggerGenWithAuth`**: Configures Swagger for API documentation with JWT bearer token authentication support.

## Installation
You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```bash
Install-Package Ethik.Utility.Api
```

Or, add the package to your .csproj file:

```bash
<PackageReference Include="Ethik.Utility.Api" Version="1.0.0" />
```

## Components

### `ApiError` Class
Represents an API error with detailed information for error handling and reporting.
#### Properties
- `ErrorCode`: A unique code identifying the error (default: "Unknown").
- `ErrorMessage`: A descriptive message about the error.
- `ErrorDescription`: A detailed description of the error.
- `ErrorSolution`: A suggested solution for resolving the error.
- `Field`: The name of the field associated with the error (if applicable).
- `Exception`: Detailed information about the exception that caused the error.
- `ExceptionObj`: The exception object used to populate the Exception property.
#### Example Usage
```csharp
var apiError = new ApiError
{
    ErrorCode = "invalid_request",
    ErrorMessage = "The request parameters are invalid.",
    Field = "username",
    ExceptionObj = new Exception("Stack trace details")
};

Console.WriteLine(apiError.ToString());
```

### `ApiExceptionDetails` Class
Represents detailed information about an exception.
#### Properties
- `Type`: The type of the exception.
- `Message`: The message associated with the exception.
- `StackTrace`: The stack trace of the exception.
- `InnerException`: The details of the inner exception, if any.

### `ApiErrorConfigService` Internal class
## Example Usage
```csharp 
// Program.cs
builder.Services.AddErrorConfig("MyApiErrors.json");
```
```json
// MyApiErrors.json
{
  "Errors": {
    "Error1": {
      "ErrorCode": "FirstError",
      "ErrorMessage": "This is first error",
      "ErrorDescription": "First error description",
      "ErrorSolution": "Solution is nothing"
    },
    "Error2": {
      "ErrorCode": "SecondError",
      "ErrorMessage": "This is second error",
      "ErrorDescription": "Second error description",
      "ErrorSolution": "Solution is nothing"
    }
  }
}
```

### `GlobalExceptionHandler` internal class
## Example Usage
```csharp 
// Program.cs

builder.Services.AddGlobalExceptionHandler();

...

var app = builder.Build();

...

app.UseExceptionHandler();

app.Run();
```

### `ApiResponse<T>` Class
A generic class to represent API responses with success or failure status.
#### Properties
- `Status`: The status of the response (Success or Failure).
- `Message`: A message describing the result of the operation.
- `StatusCode`: HTTP status code for the response.
- `Data`: The data returned by the operation (optional).
- `Errors`: List of errors related to the operation (optional).
#### Methods
- `Success(T data, int statusCode = 200, string message = "Request was successful.")`: Creates a successful response.
- `Failure(string message, int statusCode, List<ApiError>? errors = null)`: Creates a failure response with optional errors.
- `Failure(string errorKey, string message, int statusCode = 500)`: Creates a failure response using an error key (`ApiErrorCacheService` needs to be added).
#### Example Usage
```csharp
// Controller.cs
[HttpGet]
public async Task<IActionResult> ()
{
    var response = ApiResponse<int>.Success(
        32, 
        StatusCodes.Status200Ok,
        "User Age Fetched Successfully");
    return new ObjectResult(response) { StatusCode = response.StatusCode };

}
```
```csharp
// Controller.cs
/* Assuming service is already added in Program.cs */
[HttpGet]
public async Task<IActionResult> ()
{
    var response = ApiResponse<int>.Failure(
        "Error2", 
        "Cannot process request, try again later", 
        StatusCodes.Status500InternalServerError);
    return new ObjectResult(response) { StatusCode = response.StatusCode };

}
```

### `ServiceCollectionExtensions` Class
Provides extension methods for configuring services in the `IServiceCollection`.

#### Methods

- **`AddErrorConfig(IServiceCollection services, string jsonFilePath)`**:
    - **Description**: Adds and initializes the `ApiErrorConfigService` to the service collection using a JSON file for error caching.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the service to.
        - `jsonFilePath`: The file path to the JSON file used for error caching.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      // In Startup.cs or Program.cs
      services.AddErrorConfig("path/to/error-cache.json");
      ```

- **`AddGlobalExceptionHandler(IServiceCollection services)`**:
    - **Description**: Adds global exception handling and `ProblemDetails` services to the service collection.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the services to.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      // In Startup.cs or Program.cs
      services.AddGlobalExceptionHandler();
      ```

- **`AddSwaggerGenWithAuth(IServiceCollection services)`**:
    - **Description**: Adds Swagger with JWT bearer token authentication options enabled.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the services to.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      // In Startup.cs or Program.cs
      services.AddSwaggerGenWithAuth();
      ```

#### Example Usage

```csharp
// Program.cs or Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add and configure services
        services.AddErrorConfig("path/to/error-cache.json");
        services.AddJwtHelper(Configuration);
        services.AddGlobalExceptionHandler();
        services.AddSwaggerGenWithAuth();
    }

    // Other methods...
}
```
