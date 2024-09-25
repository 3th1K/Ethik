using Ethik.Utility.Data.Collections;
using Ethik.Utility.Data.Results;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Repository;

/// <summary>
/// Provides a generic base repository interface for performing CRUD operations
/// and querying entities of type <typeparamref name="T"/>. All methods return
/// an <see cref="OperationResult{T}"/> to encapsulate the result status and data.
/// </summary>
/// <typeparam name="T">The entity type that the repository manages.</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Retrieves all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> containing all entities.</returns>
    Task<OperationResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paged list of entities asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <param name="order">Optional parameter for sorting the result based on a property.</param>
    /// <param name="ascending">Determines the sorting order. Default is ascending.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{PagedList{T}}"/> containing the paginated result.</returns>
    Task<OperationResult<PagedList<T>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the entity.</returns>
    Task<OperationResult<T>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">A lambda expression to filter the entities.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> containing the matching entities.</returns>
    Task<OperationResult<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds and paginates entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <param name="predicate">A lambda expression to filter the entities.</param>
    /// <param name="order">Optional parameter for sorting the result based on a property.</param>
    /// <param name="ascending">Determines the sorting order. Default is ascending.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{PagedList{T}}"/> containing the filtered and paginated result.</returns>
    Task<OperationResult<PagedList<T>>> FindAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="autoId">Enable or disable auto ID generation, default is true.</param>
    /// <param name="customPrefix">Optional custom prefix for ID generation.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{String}"/> containing the ID of the added entity.</returns>
    Task<OperationResult<string>> AddAsync(T entity, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity with updated information.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{String}"/> indicating the result of the update operation.</returns>
    Task<OperationResult<string>> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{String}"/> indicating the result of the delete operation.</returns>
    Task<OperationResult<string>> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to soft delete.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{String}"/> indicating the result of the soft delete operation.</returns>
    Task<OperationResult<string>> SoftDeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="autoId">Enable or disable auto ID generation, default is true.</param>
    /// <param name="customPrefix">Optional custom prefix for ID generation.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{String}}"/> containing the IDs of the added entities.</returns>
    Task<OperationResult<IEnumerable<string>>> AddRangeAsync(IEnumerable<T> entities, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities with updated information.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{String}}"/> indicating the result of the update operations.</returns>
    Task<OperationResult<IEnumerable<string>>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a range of entities by their IDs asynchronously.
    /// </summary>
    /// <param name="ids">The collection of unique identifiers of the entities to delete.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{String}}"/> indicating the result of the delete operations.</returns>
    Task<OperationResult<IEnumerable<string>>> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities matching the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">A lambda expression to filter the entities.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{Int}"/> containing the count of matching entities.</returns>
    Task<OperationResult<int>> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entities matching the specified predicate exist asynchronously.
    /// </summary>
    /// <param name="predicate">A lambda expression to filter the entities.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{Boolean}"/> indicating whether any matching entities exist.</returns>
    Task<OperationResult<bool>> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}