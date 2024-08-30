# Ethik.Utility NuGet Package

## Overview

The `Ethik.Utility` NuGet package provides a set of utilities for handling common tasks in .NET applications. It includes error handling, response generation, and extensions for Entity Framework and tasks. This package is designed to simplify and standardize error management, pagination, and other utility operations in your .NET projects.

## Features

### **Database**
- **`DbSet Extensions`**:
  - **`Paged List Retrieval`**: 
    - **`GetPagedDataAsync<T>`**: Retrieves a paginated list of data from a `DbSet` with optional filtering, sorting, and pagination. Includes:
      - **Filtering**: Apply custom filtering criteria.
      - **Sorting**: Order results by a specified field in ascending or descending order.
      - **Pagination**: Controls the page number and page size for efficient data retrieval.
      - **Performance Optimization**: Uses `AsNoTracking` for better performance on read-only queries.

### **Collections**
   - **`Paged List`**: Represents a paginated list of items with metadata about the current page, total count, and navigation links.

### **Tasks**
- **`Task Extensions`**:
  - **`First Non-Null Result`**:
    - **`WhenAnyNotNull<T>`**: Returns the first successfully completed task with a non-null result from a collection of tasks or `null` if no such task exists. Features include:
      - **Error Propagation**: Handles and propagates exceptions from failed tasks.
      - **Null Handling**: Ignores tasks that complete with a `null` result.
      - **Efficiency**: Processes tasks efficiently by waiting for the first valid result or all tasks to complete.

### **API**
- **`API Responses`**: Consistent API response format for success and failure scenarios.
- **`Service Collection Extensions`**:
  - **`Error Caching`**:
    - **`AddErrorCache`**: Adds and initializes `ApiErrorCacheService` to manage error responses with configuration from a JSON file.
  - **`JWT Configuration`**:
    - **`AddJwtHelper`**: Configures JWT authentication including:
      - **Token Validation**: Validates issuer, audience, and signing key.
      - **JWT Settings**: Configures settings such as secret key, issuer, and audience from application configuration.
      - **Singleton Service**: Provides a singleton `JwtTokenService` for generating and validating tokens.
  - **`Global Exception Handling`**:
    - **`AddGlobalExceptionHandler`**: Adds global exception handling and `ProblemDetails` services for standardized error responses.
  - **`Swagger with JWT`**:
    - **`AddSwaggerGenWithAuth`**: Configures Swagger for API documentation with JWT bearer token authentication support.
### **JWT**
   - **`JWT Token Generation`**: Generates JWT tokens with customizable claims, including user details and roles.
   - **`Token Validation`**: Validates JWT tokens with support for issuer, audience, and expiration checks.
   - **`Token Expiry Configuration`**: Configurable token expiration time to control session duration.
   - **`Bearer Authentication`**: Configures JWT bearer authentication scheme for ASP.NET Core applications.
   - **`Token Details Retrieval`**: Extracts and returns token details like expiration and token type.
   - **`Singleton JWT Service`**: Provides a singleton service (JwtTokenService) for generating and validating tokens.
   - **`Custom Claims Support`**: Allows for the inclusion of custom claims in the generated JWT tokens.
   - **`Configuration-based Setup`**: Easy configuration of JWT settings (SecretKey, Issuer, Audience) through app settings.
### **Password**
   - **`Password Hashing`**: Uses the PBKDF2 algorithm to hash passwords with a randomly generated salt.
   - **`Password Verification`**: Verifies that a provided plaintext password matches a previously hashed password.
   - **`Security`**: Utilizes a 128-bit salt and a 256-bit hash for enhanced security.

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

### `ApiErrorCacheService` Internal class
## Example Usage
```csharp 
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

### `PagedList<T>` Class
A generic class to represent a paginated list of items with metadata about the current page, total count, and navigation links.
#### Properties
- `Items`: The collection of items on the current page.
- `TotalCount`: The total number of items across all pages.
- `PageNumber`: The current page number (zero-based).
- `PageSize`: The number of items per page.
- `TotalPages`: The total number of pages calculated based on TotalCount and PageSize.
- `HasPreviousPage`: A boolean indicating if there is a previous page.
- `HasNextPage`: A boolean indicating if there is a next page.
- `NextPage`: The URI for the next page (optional).
- `PreviousPage`: The URI for the previous page (optional).
#### Example Usage
```csharp
// Example of creating a PagedList instance in a service or controller
public async Task<PagedList<Product>> GetProductsAsync(int pageNumber, int pageSize)
{
    var totalProducts = await _dbContext.Products.CountAsync();
    var products = await _dbContext.Products
        .Skip(pageNumber * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var pagedProducts = new PagedList<Product>(products, totalProducts, pageNumber, pageSize);

    // Optionally set NextPage and PreviousPage URIs
    if (pagedProducts.HasNextPage)
    {
        pagedProducts.NextPage = new Uri($"/products?pageNumber={pageNumber + 1}&pageSize={pageSize}", UriKind.Relative);
    }
    if (pagedProducts.HasPreviousPage)
    {
        pagedProducts.PreviousPage = new Uri($"/products?pageNumber={pageNumber - 1}&pageSize={pageSize}", UriKind.Relative);
    }

    return pagedProducts;
}
```
```csharp
// Controller.cs
[HttpGet]
public async Task<IActionResult> GetProducts(int pageNumber = 0, int pageSize = 10)
{
    var pagedProducts = await _productService.GetProductsAsync(pageNumber, pageSize);

    return Ok(pagedProducts);
}
```

### `JwtSettings`  Class
Represents the configuration settings required for generating JWT tokens.
#### Properties
- `SecretKey`: The secret key used for signing the JWT token.
- `Issuer`: The issuer of the JWT token.
- `Audience`: The audience for which the JWT token is intended.
- `ExpiryMinutes`: The expiration time in minutes for the JWT token.

### `JwtTokenResponse` Class
Represents the response containing details about a generated JWT token.
#### Properties
- `Token`: The JWT token as a string.
- `Expiration`: The expiration time of the JWT token.
- `TokenType`: The type of token, typically "Bearer".
#### Example Usage
```csharp
// Example of using JwtTokenResponse
var tokenResponse = new JwtTokenResponse(token, DateTime.UtcNow.AddMinutes(30), "Bearer");

Console.WriteLine($"Token: {tokenResponse.Token}");
Console.WriteLine($"Expires At: {tokenResponse.Expiration}");
Console.WriteLine($"Token Type: {tokenResponse.TokenType}");
```

### `JwtTokenService` Class
Provides functionality for generating and validating JWT tokens.
#### Methods
- `GenerateToken(string userId, string email, string role)`: Generates a JWT token based on the provided user details. Returns a string representation of the generated JWT token.
- `GetTokenDetails(string token)`: Retrieves details about the provided JWT token. Returns a `JwtTokenResponse` object containing token details such as expiration and type.
- `TokenType`: The type of token, typically "Bearer".
#### Example Usage
```csharp
// Example of generating a JWT token
public async Task<string> GenerateUserToken()
{
    var tokenService = new JwtTokenService(_options); // _options injected via DI
    var token = tokenService.GenerateToken("123", "user@example.com", "User");
    return token;
}

// Example of retrieving token details
public async Task<JwtTokenResponse> GetUserTokenDetails(string token)
{
    var tokenService = new JwtTokenService(_options); // _options injected via DI
    var tokenDetails = tokenService.GetTokenDetails(token);
    return tokenDetails;
}
```

### `PasswordHasher` Class
A static class that provides functionality for hashing and verifying passwords using the PBKDF2 algorithm with a randomly generated salt.
#### Methods
- `HashPassword(string password)`: Hashes a password using the PBKDF2 algorithm with a randomly generated salt. Returns a string containing the salt and hashed password, separated by a dot.
- `VerifyPassword(string hashedPassword, string providedPassword)`: Verifies that a provided password matches a previously hashed password. Returns `True` if the provided password matches the hashed password; otherwise, `False`.
#### Example Usage
```csharp
// Hashing a password
string plaintextPassword = "MySecurePassword";
string hashedPassword = PasswordHasher.HashPassword(plaintextPassword);
Console.WriteLine($"Hashed Password: {hashedPassword}");

```
```csharp
// Verifying a password
string storedHashedPassword = "stored-salt.hash";
string providedPassword = "MySecurePassword";

bool isPasswordValid = PasswordHasher.VerifyPassword(storedHashedPassword, providedPassword);

if (isPasswordValid)
{
    Console.WriteLine("Password is valid.");
}
else
{
    Console.WriteLine("Invalid password.");
}

```

### `ServiceCollectionExtensions` Class
Provides extension methods for configuring services in the `IServiceCollection`.

#### Methods

- **`AddErrorCache(IServiceCollection services, string jsonFilePath)`**:
    - **Description**: Adds and initializes the `ApiErrorCacheService` to the service collection using a JSON file for error caching.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the service to.
        - `jsonFilePath`: The file path to the JSON file used for error caching.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      // In Startup.cs or Program.cs
      services.AddErrorCache("path/to/error-cache.json");
      ```

- **`AddJwtHelper(IServiceCollection services, IConfiguration configuration)`**:
    - **Description**: Configures JWT authentication and adds the `JwtTokenService` to the service collection.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the services to.
        - `configuration`: The application configuration containing JWT settings.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      // In Startup.cs or Program.cs
      services.AddJwtHelper(Configuration);
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

#### Example Configuration

```csharp
// Program.cs or Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add and configure services
        services.AddErrorCache("path/to/error-cache.json");
        services.AddJwtHelper(Configuration);
        services.AddGlobalExceptionHandler();
        services.AddSwaggerGenWithAuth();
    }

    // Other methods...
}
```

### `DbSetExtensions` Class
Provides extension methods for working with `DbSet<TEntity>` in Entity Framework.

#### Methods

- **`GetPagedDataAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? order = null, bool ascending = true, int pageNumber = 1, int pageSize = 10)`**:
    - **Description**: Retrieves a paginated list of data from the specified `DbSet`, with optional filtering, sorting, and pagination.
    - **Type Parameters**:
        - `T`: The type of the entity.
    - **Parameters**:
        - `dbSet`: The `DbSet` to retrieve data from.
        - `filter`: An optional expression to filter the data.
        - `order`: An optional expression to order the data by.
        - `ascending`: Determines whether the data should be sorted in ascending or descending order. Default is `true` (ascending).
        - `pageNumber`: The page number to retrieve. Default is `1`.
        - `pageSize`: The number of items per page. Default is `10`.
    - **Returns**: A `PagedList<T>` containing the paginated data.
    - **Example Usage**:
      ```csharp
      // In a repository or service class
      public async Task<PagedList<MyEntity>> GetPagedEntitiesAsync(int pageNumber, int pageSize)
      {
          // Define filter and sorting expressions
          Expression<Func<MyEntity, bool>> filter = e => e.IsActive;
          Expression<Func<MyEntity, object>> order = e => e.CreatedDate;

          // Retrieve paginated data
          var pagedData = await dbContext.MyEntities.GetPagedDataAsync(
              filter: filter,
              order: order,
              ascending: true,
              pageNumber: pageNumber,
              pageSize: pageSize
          );

          return pagedData;
      }
      ```

#### Example Configuration

```csharp
// In a service or repository
public class MyEntityService
{
    private readonly DbContext _dbContext;

    public MyEntityService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedList<MyEntity>> GetPagedEntitiesAsync(int pageNumber, int pageSize)
    {
        // Define filter and sorting expressions
        Expression<Func<MyEntity, bool>> filter = e => e.IsActive;
        Expression<Func<MyEntity, object>> order = e => e.CreatedDate;

        // Retrieve paginated data
        var pagedData = await _dbContext.MyEntities.GetPagedDataAsync(
            filter: filter,
            order: order,
            ascending: true,
            pageNumber: pageNumber,
            pageSize: pageSize
        );

        return pagedData;
    }
}
```

### `TaskExtensions` Class
Provides extension methods for working with tasks.

#### Methods

- **`WhenAnyNotNull<T>(this IEnumerable<Task<T?>> tasks)`**:
    - **Description**: Returns the first successfully completed task that is not null from a collection of tasks, or null if no such task exists.
    - **Type Parameters**:
        - `T`: The type of the task result. Must be a class (reference type).
    - **Parameters**:
        - `tasks`: A collection of tasks to monitor.
    - **Returns**: 
        - A `Task<T?>` that represents the first successfully completed task with a non-null result. If all tasks complete without a non-null result, returns `null`.
    - **Example Usage**:
      ```csharp
      // In a service or method where you want to get the first non-null result
      public async Task<MyEntity?> GetFirstNonNullResultAsync()
      {
          // Define a collection of tasks
          var tasks = new List<Task<MyEntity?>>
          {
              Task.Run(() => (MyEntity?)null), // Task that returns null
              Task.Run(() => new MyEntity { Id = 1 }), // Task that returns a non-null result
              Task.Run(() => (MyEntity?)null) // Another task that returns null
          };

          // Get the first non-null result
          var result = await tasks.WhenAnyNotNull();

          return result;
      }
      ```

#### Example Configuration

```csharp
// Define your class and method that uses the extension method
public class MyService
{
    // Example entity class
    public class MyEntity
    {
        public int Id { get; set; }
    }

    public async Task<MyEntity?> GetFirstNonNullResultAsync()
    {
        // Define a collection of tasks
        var tasks = new List<Task<MyEntity?>>
        {
            Task.Run(() => (MyEntity?)null), // Task that returns null
            Task.Run(() => new MyEntity { Id = 1 }), // Task that returns a non-null result
            Task.Run(() => (MyEntity?)null) // Another task that returns null
        };

        // Get the first non-null result
        var result = await tasks.WhenAnyNotNull();

        return result;
    }
}
```

## Contributing
Contributions are welcome! Please open an issue or submit a pull request with your changes.
## License
This package is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

