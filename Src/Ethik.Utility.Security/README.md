# Ethik.Utility.Security NuGet Package

## Overview
The `Ethik.Utility.Security` NuGet package provides a set of utilities for handling security tasks in .NET applications. It includes tools to deal with passwords, token managements, and required extensions.

## **Features**
- **`Password`**: Tools to hash password and verify hashed passwords.
- **`JWT Service`**: Tool to Generate jwt token and extract token details.
  - **`Service Collection Extensions`**: Extensins provided to add jwt in pipeline
	- **`AddJwtHelper`**: Adds JWT token services and settings.

## Installation
You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```bash
Install-Package Ethik.Utility.Security
```

Or, add the package to your .csproj file:

```bash
<PackageReference Include="Ethik.Utility.Security" Version="1.0.0" />
```

## Components

### `JwtTokenService` Class
Provides functionality for generating and validating JWT tokens for authentication purposes.

#### Properties
- **`_jwtSettings`**: Stores the JWT settings (`JwtSettings`) used to configure the token generation process. This includes values like the secret key, issuer, audience, and token expiry time.

#### Methods
- **`GenerateToken(string userId, string email, string role)`**:
    - **Description**: Generates a JWT token based on the provided user details, including user ID, email, and role.
    - **Parameters**:
        - `userId`: The unique identifier of the user.
        - `email`: The user's email address.
        - `role`: The user's role (e.g., Admin, User).
    - **Returns**: A string representation of the generated JWT token.
    - **Example Usage**:
      ```csharp
      var jwtService = new JwtTokenService(options);
      var token = jwtService.GenerateToken("123", "user@example.com", "Admin");
      Console.WriteLine(token);
      ```

- **`GetTokenDetails(string token)`**:
    - **Description**: Extracts details from the provided JWT token, including expiration time and token type.
    - **Parameters**:
        - `token`: The JWT token to extract details from.
    - **Returns**: A `JwtTokenDetails` object containing the expiration time and token type.
    - **Example Usage**:
      ```csharp
      var jwtService = new JwtTokenService(options);
      var tokenDetails = jwtService.GetTokenDetails(token);
      Console.WriteLine($"Token expires at: {tokenDetails.Expiration}");
      ```

---

### `JwtTokenDetails` Class
Represents details about a JWT token, including expiration time and token type.

#### Properties
- **`Token`**: The JWT token string.
- **`Expiration`**: The expiration time of the token.
- **`TokenType`**: The type of the token (e.g., "Bearer").

---

### `JwtSettings` Class
Stores configuration settings required for generating JWT tokens.

#### Properties
- **`Issuer`**: The issuer of the JWT token.
- **`Audience`**: The audience for the JWT token.
- **`SecretKey`**: The secret key used to sign the JWT token.
- **`ExpiryMinutes`**: The number of minutes until the token expires.

---

### `ServiceCollectionExtensions` Class
Provides extension methods for configuring JWT authentication and adding the `JwtTokenService` to the service collection.

#### Methods

- **`AddJwtHelper(IServiceCollection services, IConfiguration configuration)`**:
    - **Description**: Configures JWT authentication and registers the `JwtTokenService` in the service collection.
    - **Parameters**:
        - `services`: The `IServiceCollection` to add the JWT authentication to.
        - `configuration`: The application's configuration section containing JWT settings.
    - **Returns**: The updated `IServiceCollection`.
    - **Example Usage**:
      ```csharp
      public class Startup
      {
          public void ConfigureServices(IServiceCollection services)
          {
              services.AddJwtHelper(Configuration);
          }
      }
      ```

#### Example Usage

```csharp
// In Program.cs or Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add JWT authentication and helper service.
        services.AddJwtHelper(Configuration);
    }

    // Other methods...
}
```

---

### `JwtSettings` Configuration Example
Here's an example configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "super_secret_key_12345",
    "Issuer": "MyApp",
    "Audience": "MyAppAudience",
    "ExpiryMinutes": 60
  }
}
```

In your `Program.cs` or `Startup.cs`, you would load the configuration and set up JWT authentication as shown in the examples above.