# Ethik.Utility NuGet Package

## Overview

The `Ethik.Utility` NuGet package provides a set of utilities for handling common tasks in .NET applications. It includes error handling, response generation, and extensions for Entity Framework and tasks. This package is designed to simplify and standardize error management, pagination, and other utility operations in your .NET projects.

## Features
- ### API
    - `Global Error Handling`:  Centralized exception handling with detailed error responses.
    - `API Responses`: Consistent API response format for success and failure scenarios.
    - `Error Configuration/Error Caching`: Configuration-based error handling with dynamic reloading and caching.

## Installation
You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```bash
Install-Package Ethik.Utility
```

Or, add the package to your .csproj file:

```bash
<PackageReference Include="Ethik.Utility" Version="1.0.0" />
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
#### Example
```c#
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
#### Example
```c# 
// Program.cs
builder.Services.AddErrorCache("MyApiErrors.json");
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
```c#
// Controller.cs
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

### `ApiErrorCacheService` internal class
### `GlobalExceptionHandler` internal class





## Contributing
Contributions are welcome! Please open an issue or submit a pull request with your changes.
## License
This package is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

