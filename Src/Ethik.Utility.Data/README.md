# Ethik.Utility.Data NuGet Package

## Overview
The `Ethik.Utility.Data` NuGet package provides a set of utilities for handling Database related tasks in .NET applications.

## Installation
You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```bash
Install-Package Ethik.Utility.Data
```

Or, add the package to your .csproj file:

```bash
<PackageReference Include="Ethik.Utility.Data" Version="1.0.0" />
```

## Components

### `OperationError` Class
Represents an error that occurs during an operation, supporting an error message, exception, depth, and error code.

#### Properties
- **`ErrorMessage`**: The message describing the error that occurred during the operation.
- **`ErrorCode`**: The code associated with the error, which can be used for categorization or identification of the error type.
- **`Exception`**: The exception that caused the operation error, if applicable. This can be `null` if no exception is associated.
- **`Depth`**: The depth of the error as it propagates across different layers of the application.

#### Methods
- **`IncrementDepth()`**:
    - **Description**: Increases the depth of the error as it gets propagated across layers.
    - **Example Usage**:
      ```csharp
      var error = new OperationError("An error occurred.", "ERR001", 1);
      error.IncrementDepth();
      Console.WriteLine(error.Depth); // Outputs: 2
      ```

---

### `OperationResult<T>` Class
Represents the result of an operation, either successful or failed, with support for stacking errors and metadata.

#### Properties
- **`Data`**: The result data returned by the operation, or `null` if the operation failed.
- **`IsSuccess`**: A boolean indicating whether the operation was successful.
- **`ErrorStack`**: A list of `OperationError` objects representing the errors encountered during the operation.
- **`Metadata`**: A dictionary for storing additional data related to the operation result.

#### Methods
- **`Success(T data)`**:
    - **Description**: Creates a successful operation result with the provided data.
    - **Parameters**:
        - `data`: The data returned by the operation.
    - **Returns**: An `OperationResult<T>` instance representing a successful result.
    - **Example Usage**:
      ```csharp
      var result = OperationResult<string>.Success("Operation completed successfully.");
      Console.WriteLine(result.IsSuccess); // Outputs: True
      ```

- **`Failure(string errorMessage, string errorCode)`**:
    - **Description**: Creates a failed operation result using an error message and error code.
    - **Parameters**:
        - `errorMessage`: The message describing the error.
        - `errorCode`: The code associated with the error.
    - **Returns**: An `OperationResult<T>` instance representing a failed result.
    - **Example Usage**:
      ```csharp
      var result = OperationResult<string>.Failure("An error occurred.", "ERR002");
      Console.WriteLine(result.IsSuccess); // Outputs: False
      ```

- **`Failure(Exception exception, string errorCode)`**:
    - **Description**: Creates a failed operation result using an exception and an error code.
    - **Parameters**:
        - `exception`: The exception that caused the failure.
        - `errorCode`: The code associated with the error.
    - **Returns**: An `OperationResult<T>` instance representing a failed result.
    - **Example Usage**:
      ```csharp
      var result = OperationResult<string>.Failure(new Exception("Exception message"), "ERR003");
      Console.WriteLine(result.IsSuccess); // Outputs: False
      ```

- **`AddError(string errorMessage, string errorCode, Exception? exception = null)`**:
    - **Description**: Adds a new error with the specified message and increments the depth.
    - **Parameters**:
        - `errorMessage`: The message describing the error.
        - `errorCode`: The code associated with the error.
        - `exception`: The optional exception that caused the error.
    - **Example Usage**:
      ```csharp
      var result = OperationResult<string>.Failure("Initial error.", "ERR004");
      result.AddError("Additional error occurred.", "ERR005");
      Console.WriteLine(result.ErrorStack.Count); // Outputs: 2
      ```

#### Example Usage
```csharp
var successResult = OperationResult<string>.Success("Data loaded successfully.");
var failureResult = OperationResult<string>.Failure("Failed to load data.", "LOAD_ERR");

// Adding an error to the failure result
failureResult.AddError("Database connection lost.", "DB_ERR");

// Checking results
Console.WriteLine(successResult.IsSuccess); // Outputs: True
Console.WriteLine(failureResult.IsSuccess); // Outputs: False
```

--- 

### Example Configuration
The `OperationResult<T>` class can be used in various contexts, such as API responses, service operations, or any place where you need to encapsulate the outcome of an operation with potential errors.

## IBaseEntity Interface

The `IBaseEntity` interface represents the base structure for an entity within the system. It includes properties for managing entity identity, creation, modification, and deletion status.

### Properties

- **Id**  
  Gets or sets the unique identifier for the entity.  
  **Type:** `string`  
  **Example:**  
  ```csharp
  public class User : IBaseEntity
  {
      public string Id { get; set; }
      // other properties
  }
  ```

- **Created**  
  Gets or sets the date and time when the entity was created.  
  **Type:** `DateTime`  
  **Example:**  
  ```csharp
  public DateTime Created { get; set; } = DateTime.UtcNow;
  ```

- **LastModified**  
  Gets or sets the date and time when the entity was last modified.  
  **Type:** `DateTime`  
  **Example:**  
  ```csharp
  public DateTime LastModified { get; set; } = DateTime.UtcNow;
  ```

- **IsDeleted**  
  Gets or sets a value indicating whether the entity is marked as deleted.  
  **Type:** `bool?`  
  **Example:**  
  ```csharp
  public bool? IsDeleted { get; set; } = false;
  ```

## IBaseRepository<T> Interface

The `IBaseRepository<T>` interface provides a generic contract for a repository that allows for CRUD (Create, Read, Update, Delete) operations and querying of entities of type `T`. All methods return an `OperationResult<T>` to encapsulate the result status and data.

### Type Parameters

- **T**  
  The entity type that the repository manages. Must be a class.

### Methods

- **GetAllAsync**  
  Retrieves all entities asynchronously.  
  **Parameters:**  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<IEnumerable<T>>>`  
  **Example:**  
  ```csharp
  var result = await repository.GetAllAsync();
  if (result.Success)
  {
      var entities = result.Data;
  }
  ```

- **GetAllAsync (Paged)**  
  Retrieves a paged list of entities asynchronously.  
  **Parameters:**  
  - `int pageNumber`: The page number to retrieve.  
  - `int pageSize`: The number of entities per page.  
  - `Expression<Func<T, object>>? order` (optional): A lambda expression for sorting.  
  - `bool ascending` (default is `true`): Sort order.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<PagedList<T>>>`  
  **Example:**  
  ```csharp
  var pagedResult = await repository.GetAllAsync(1, 10);
  ```

- **GetByIdAsync**  
  Retrieves a single entity by its ID asynchronously.  
  **Parameters:**  
  - `string id`: The unique identifier of the entity.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<T>>`  
  **Example:**  
  ```csharp
  var result = await repository.GetByIdAsync("entityId");
  ```

- **FindAsync**  
  Finds entities that match the specified predicate asynchronously.  
  **Parameters:**  
  - `Expression<Func<T, bool>> predicate`: A lambda expression to filter entities.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<IEnumerable<T>>>`  
  **Example:**  
  ```csharp
  var result = await repository.FindAsync(x => x.IsDeleted == false);
  ```

- **AddAsync**  
  Adds a new entity asynchronously.  
  **Parameters:**  
  - `T entity`: The entity to add.  
  - `bool autoId` (default is `true`): Enable or disable auto ID generation.  
  - `string? customPrefix` (optional): Custom prefix for ID generation.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<string>>`  
  **Example:**  
  ```csharp
  var newEntity = new User { /* properties */ };
  var result = await repository.AddAsync(newEntity);
  ```

- **UpdateAsync**  
  Updates an existing entity asynchronously.  
  **Parameters:**  
  - `T entity`: The entity with updated information.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<string>>`  
  **Example:**  
  ```csharp
  var updatedEntity = new User { Id = "entityId", /* updated properties */ };
  var result = await repository.UpdateAsync(updatedEntity);
  ```

- **DeleteAsync**  
  Deletes an entity by its ID asynchronously.  
  **Parameters:**  
  - `string id`: The unique identifier of the entity to delete.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<string>>`  
  **Example:**  
  ```csharp
  var result = await repository.DeleteAsync("entityId");
  ```

- **SoftDeleteAsync**  
  Soft deletes an entity by its ID asynchronously.  
  **Parameters:**  
  - `string id`: The unique identifier of the entity to soft delete.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<string>>`  
  **Example:**  
  ```csharp
  var result = await repository.SoftDeleteAsync("entityId");
  ```

- **AddRangeAsync**  
  Adds a range of entities asynchronously.  
  **Parameters:**  
  - `IEnumerable<T> entities`: The collection of entities to add.  
  - `bool autoId` (default is `true`): Enable or disable auto ID generation.  
  - `string? customPrefix` (optional): Custom prefix for ID generation.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<IEnumerable<string>>>`  
  **Example:**  
  ```csharp
  var entitiesToAdd = new List<User> { /* entity instances */ };
  var result = await repository.AddRangeAsync(entitiesToAdd);
  ```

- **UpdateRangeAsync**  
  Updates a range of entities asynchronously.  
  **Parameters:**  
  - `IEnumerable<T> entities`: The collection of entities with updated information.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<IEnumerable<string>>>`  
  **Example:**  
  ```csharp
  var entitiesToUpdate = new List<User> { /* updated entity instances */ };
  var result = await repository.UpdateRangeAsync(entitiesToUpdate);
  ```

- **DeleteRangeAsync**  
  Deletes a range of entities by their IDs asynchronously.  
  **Parameters:**  
  - `IEnumerable<string> ids`: The collection of unique identifiers of the entities to delete.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<IEnumerable<string>>>`  
  **Example:**  
  ```csharp
  var idsToDelete = new List<string> { "entityId1", "entityId2" };
  var result = await repository.DeleteRangeAsync(idsToDelete);
  ```

- **CountAsync**  
  Counts the number of entities matching the specified predicate asynchronously.  
  **Parameters:**  
  - `Expression<Func<T, bool>> predicate`: A lambda expression to filter entities.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<int>>`  
  **Example:**  
  ```csharp
  var countResult = await repository.CountAsync(x => x.IsDeleted == false);
  ```

- **ExistsAsync**  
  Checks if any entities matching the specified predicate exist asynchronously.  
  **Parameters:**  
  - `Expression<Func<T, bool>> predicate`: A lambda expression to filter entities.  
  - `CancellationToken cancellationToken` (optional): A token to cancel the operation.  
  **Returns:** `Task<OperationResult<bool>>`  
  **Example:**  
  ```csharp
  var existsResult = await repository.ExistsAsync(x => x.Id == "entityId");
  ```

## Components

### `PagedList<T>` Class
Represents a paginated list of items with metadata about the current page, total count, and navigation links.

#### Properties
- **`Items`**: 
  - **Description**: Gets or sets the collection of items on the current page.
  - **Type**: `IEnumerable<T>`

- **`TotalCount`**: 
  - **Description**: Gets or sets the total number of items across all pages.
  - **Type**: `int`

- **`PageNumber`**: 
  - **Description**: Gets or sets the current page number (zero-based).
  - **Type**: `int`

- **`PageSize`**: 
  - **Description**: Gets or sets the number of items per page.
  - **Type**: `int`

- **`TotalPages`**: 
  - **Description**: Gets the total number of pages.
  - **Type**: `int`
  - **Calculated**: `Math.Ceiling((double)TotalCount / PageSize)`

- **`HasPreviousPage`**: 
  - **Description**: Gets a value indicating whether there is a previous page.
  - **Type**: `bool`

- **`HasNextPage`**: 
  - **Description**: Gets a value indicating whether there is a next page.
  - **Type**: `bool`

- **`NextPage`**: 
  - **Description**: Gets or sets the URI for the next page, if applicable.
  - **Type**: `Uri?`
  - **JsonIgnore**: Condition is set to ignore when writing null.

- **`PreviousPage`**: 
  - **Description**: Gets or sets the URI for the previous page, if applicable.
  - **Type**: `Uri?`
  - **JsonIgnore**: Condition is set to ignore when writing null.

#### Constructor
- **`PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)`**:
  - **Description**: Initializes a new instance of the `PagedList<T>` class with the specified items and pagination metadata.
  - **Parameters**:
    - `items`: The collection of items on the current page.
    - `totalCount`: The total number of items across all pages.
    - `pageNumber`: The current page number (zero-based).
    - `pageSize`: The number of items per page.

#### Methods
- **`GetEnumerator()`**:
  - **Description**: Returns an enumerator that iterates through the collection of items on the current page.
  - **Returns**: An enumerator that can be used to iterate through the collection of items.

#### Example Usage
```csharp
// Example of creating a PagedList
var items = new List<string> { "Item1", "Item2", "Item3" };
var pagedList = new PagedList<string>(items, totalCount: 100, pageNumber: 0, pageSize: 3);

// Accessing properties
Console.WriteLine($"Total Pages: {pagedList.TotalPages}");
Console.WriteLine($"Has Next Page: {pagedList.HasNextPage}");
```

### `DictionaryExtensions` Class
Provides extension methods for the `Dictionary<TKey, TValue>` class.

#### Methods

- **`AddRange(this Dictionary<string, object> target, Dictionary<string, object>? source)`**:
    - **Description**: Adds the elements of the specified dictionary to the current dictionary.
    - **Parameters**:
        - `target`: The target dictionary to which elements will be added.
        - `source`: The source dictionary containing elements to add. Can be null.
    - **Returns**: `void`.
    - **Example Usage**:
      ```csharp
      var targetDictionary = new Dictionary<string, object>();
      var sourceDictionary = new Dictionary<string, object>
      {
          { "Key1", "Value1" },
          { "Key2", "Value2" }
      };
      targetDictionary.AddRange(sourceDictionary);
      ```

---

### `DbSetExtensions` Class
Provides extension methods for working with `DbSet<TEntity>` in Entity Framework.

#### Methods

- **`GetPagedDataAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? order = null, bool ascending = true, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)`**:
    - **Description**: Retrieves a paginated list of data from the specified DbSet, with optional filtering, sorting, and pagination.
    - **Type Parameters**:
        - `T`: The type of the entity.
    - **Parameters**:
        - `dbSet`: The DbSet to retrieve data from.
        - `filter`: An optional expression to filter the data.
        - `order`: An optional expression to order the data by.
        - `ascending`: Determines whether the data should be sorted in ascending or descending order. Default is true (ascending).
        - `pageNumber`: The page number to retrieve. Default is 1.
        - `pageSize`: The number of items per page. Default is 10.
        - `cancellationToken`: A cancellation token to observe while waiting for the task to complete.
    - **Returns**: A `PagedList<T>` containing the paginated data.
    - **Example Usage**:
      ```csharp
      var pagedData = await dbSet.GetPagedDataAsync(filter: x => x.IsActive, order: x => x.Name);
      ```

---

### `DbContextExtensions` Class
Provides extension methods for `DbContext` to offer additional functionalities such as auto-generating IDs with optional prefixes.

#### Methods

- **`AddEntityWithAutoIdAsync<TEntity>(this DbContext context, TEntity entity, Expression<Func<TEntity, object>> idPropertyExpression, string? customPrefix = null, CancellationToken cancellationToken = default)`**:
    - **Description**: Adds an entity to the `DbContext` with an auto-generated ID. The ID is generated based on the specified property and an optional custom prefix.
    - **Type Parameters**:
        - `TEntity`: The type of the entity.
    - **Parameters**:
        - `context`: The `DbContext` in which the entity will be added.
        - `entity`: The entity to be added.
        - `idPropertyExpression`: Expression to access the ID property of the entity.
        - `customPrefix`: Optional custom prefix for the ID. If not provided, a prefix is generated from the entity type name.
        - `cancellationToken`: A cancellation token to observe while waiting for the task to complete.
    - **Returns**: A `Task` representing the asynchronous operation.
    - **Example Usage**:
      ```csharp
      await dbContext.AddEntityWithAutoIdAsync(myEntity, x => x.Id);
      ```

- **`AddEntitiesWithAutoIdAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idPropertyExpression, string? customPrefix = null, CancellationToken cancellationToken = default)`**:
    - **Description**: Adds multiple entities to the `DbContext` with auto-generated IDs. The IDs are generated based on the specified property and an optional custom prefix.
    - **Type Parameters**:
        - `TEntity`: The type of the entity.
    - **Parameters**:
        - `context`: The `DbContext` in which the entities will be added.
        - `entities`: The list of entities to be added.
        - `idPropertyExpression`: Expression to access the ID property of the entity.
        - `customPrefix`: Optional custom prefix for the IDs. If not provided, a prefix is generated from the entity type name.
        - `cancellationToken`: A cancellation token to observe while waiting for the task to complete.
    - **Returns**: A `Task` representing the asynchronous operation.
    - **Example Usage**:
      ```csharp
      await dbContext.AddEntitiesWithAutoIdAsync(myEntities, x => x.Id);
      ```

#### Private Methods

- **`SetId<TEntity>(TEntity entity, Expression<Func<TEntity, object>> idPropertyExpression, string? customPrefix = null)`**:
    - **Description**: Sets an ID for the specified entity using a given property and an optional custom prefix.
    - **Parameters**:
        - `entity`: The entity for which the ID is to be set.
        - `idPropertyExpression`: Expression to access the ID property of the entity.
        - `customPrefix`: Optional prefix to use for the generated ID.
    - **Returns**: `void`.
    - **Example Usage**: This method is called internally and does not need to be called directly by the user.

- **`GetMemberExpression<TEntity>(Expression<Func<TEntity, object>> expression)`**:
    - **Description**: Extracts the `MemberExpression` from a given property expression.
    - **Parameters**:
        - `expression`: The expression representing the property.
    - **Returns**: The `MemberExpression` or null if the expression is not valid.

- **`GeneratePrefixFromTypeName(string typeName)`**:
    - **Description**: Generates a prefix from the entity type name, by taking the first 3 characters of the type name and converting them to uppercase.
    - **Parameters**:
        - `typeName`: The name of the entity type.
    - **Returns**: A string containing the first 3 uppercase characters of the type name.

- **`GenerateIdWithPrefix(string prefix)`**:
    - **Description**: Generates an ID with the given prefix. The ID is a combination of the prefix and the current timestamp.
    - **Parameters**:
        - `prefix`: The prefix to be used in the ID.
    - **Returns**: A string representing the generated ID.

---

### Example Usage for Configuration

You may want to configure services and usage in your application like this:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    }
}
```

  
# BaseRepository

## Overview

The `BaseRepository<T, TContext>` class provides standard CRUD (Create, Read, Update, Delete) operations for managing entities in a .NET application. It serves as a base class for specific repository implementations that work with a `DbContext` of type `TContext`.

## **Features**
- **CRUD Operations**: Implements basic CRUD operations such as `Add`, `Delete`, and `Find`.
- **Batch Operations**: Supports adding and deleting multiple entities in a single call.
- **Count and Existence Check**: Provides methods to count entities and check for existence based on a predicate.

## Type Parameters
- **T**: The type of the entity that the repository will manage. It must implement the `IBaseEntity` interface.
- **TContext**: The type of the `DbContext` that the repository will use. It must inherit from `DbContext`.

## Constructors

### BaseRepository(IDbContextFactory<TContext> contextFactory, ILogger logger)

Initializes a new instance of the `BaseRepository<T, TContext>` class.

#### Parameters
- **contextFactory**: The factory used to create the `DbContext`.
- **logger**: The logger instance for logging operations.

## Methods

### Task<OperationResult<string>> AddAsync(T entity, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default)

Adds a new entity asynchronously.

#### Parameters
- **entity**: The entity to add.
- **autoId** (optional): A boolean indicating whether to enable automatic ID generation. The default value is `true`.
- **customPrefix** (optional): An optional custom prefix for ID generation.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<string>` containing the ID of the added entity.

---

### Task<OperationResult<IEnumerable<string>>> AddRangeAsync(IEnumerable<T> entities, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default)

Adds a range of entities asynchronously.

#### Parameters
- **entities**: The entities to add.
- **autoId** (optional): A boolean indicating whether to enable automatic ID generation. The default value is `true`.
- **customPrefix** (optional): An optional custom prefix for ID generation.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<IEnumerable<string>>` containing the IDs of the added entities.

---

### Task<OperationResult<int>> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)

Counts the number of entities that satisfy the specified predicate asynchronously.

#### Parameters
- **predicate**: An expression used to filter entities.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<int>` containing the count of matching entities.

---

### Task<OperationResult<string>> DeleteAsync(string id, CancellationToken cancellationToken = default)

Deletes an entity by its ID asynchronously.

#### Parameters
- **id**: The ID of the entity to delete.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<string>` containing the ID of the deleted entity.

---

### Task<OperationResult<IEnumerable<string>>> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)

Deletes a range of entities by their IDs asynchronously.

#### Parameters
- **ids**: The IDs of the entities to delete.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<IEnumerable<string>>` containing the IDs of the deleted entities.

---

### Task<OperationResult<bool>> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)

Checks if any entities exist that satisfy the specified predicate asynchronously.

#### Parameters
- **predicate**: An expression used to filter entities.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<bool>` indicating whether any entities exist.

---

### Task<OperationResult<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)

Finds entities that satisfy the specified predicate asynchronously.

#### Parameters
- **predicate**: An expression used to filter entities.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<IEnumerable<T>>` containing the matching entities.

---

### Task<OperationResult<T?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)

Retrieves an entity by its ID asynchronously.

#### Parameters
- **id**: The ID of the entity to retrieve.
- **cancellationToken** (optional): A token to cancel the operation.

#### Returns
An `OperationResult<T?>` containing the retrieved entity or null if not found.

---

## Example Usage

```csharp
public class UserRepository : BaseRepository<User, ApplicationDbContext>
{
    public UserRepository(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<UserRepository> logger)
        : base(contextFactory, logger)
    {
    }

    // Additional repository methods specific to User entity can be added here
}
```

### Adding a New User

```csharp
var user = new User { Name = "John Doe", Email = "john@example.com" };
var result = await userRepository.AddAsync(user);

if (result.IsSuccess)
{
    Console.WriteLine($"User added with ID: {result.Value}");
}
```

### Finding a User by ID

```csharp
var userId = "user-id";
var findResult = await userRepository.GetByIdAsync(userId);

if (findResult.IsSuccess && findResult.Value != null)
{
    var user = findResult.Value;
    Console.WriteLine($"Found User: {user.Name}");
}
```