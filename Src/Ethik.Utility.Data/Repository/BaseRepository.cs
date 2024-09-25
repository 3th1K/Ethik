using Ethik.Utility.Data.Collections;
using Ethik.Utility.Data.Extensions;
using Ethik.Utility.Data.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Repository;

/// <summary>
/// Represents a base repository that provides standard CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="TContext">The type of the DbContext.</typeparam>
public abstract partial class BaseRepository<T, TContext> : IBaseRepository<T>
    where T : class, IBaseEntity
    where TContext : DbContext
{
    protected readonly IDbContextFactory<TContext> _contextFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{T, TContext}"/> class.
    /// </summary>
    /// <param name="contextFactory">The factory to create the DbContext.</param>
    protected BaseRepository(IDbContextFactory<TContext> contextFactory, ILogger logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="autoId">Enable or disable auto ID generation, default is true.</param>
    /// <param name="customPrefix">Optional custom prefix for ID generation.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="OperationResult{String}"/> containing the ID of the added entity.</returns>
    public virtual async Task<OperationResult<string>> AddAsync(T entity, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            entity.Created = entity.LastModified = DateTime.UtcNow;
            if (autoId)
            {
                await context.AddEntityWithAutoIdAsync(entity, e => e.Id, customPrefix, cancellationToken);
            }
            else
            {
                await context.Set<T>().AddAsync(entity, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Added entity {@Name}: {$Obj}", typeof(T).Name, entity.Id);
            return OperationResult<string>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to add entity");
            return OperationResult<string>.Failure("Unable to add entity", ex, RepositoryErrorCodes.AddEntityFailure);
        }
    }

    /// <summary>
    /// Adds a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the IDs of the added entities.</returns>
    public virtual async Task<OperationResult<IEnumerable<string>>> AddRangeAsync(IEnumerable<T> entities, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            foreach (var entity in entities)
            {
                entity.Created = entity.LastModified = DateTime.UtcNow;
            }
            if (autoId)
            {
                await context.AddEntitiesWithAutoIdAsync(entities, e => e.Id, customPrefix, cancellationToken);
            }
            else
            {
                await context.Set<T>().AddRangeAsync(entities, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            var ids = entities.Select(e => e.Id);
            _logger.LogDebug("Added entities {@Name}: {$Obj}", typeof(T).Name, ids);
            return OperationResult<IEnumerable<string>>.Success(ids);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to add entities");
            return OperationResult<IEnumerable<string>>.Failure("Unable to add entities", ex, RepositoryErrorCodes.AddEntitiesFailure);
        }
    }

    /// <summary>
    /// Counts the number of entities that satisfy the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the count of matching entities.</returns>
    public virtual async Task<OperationResult<int>> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var count = await context.Set<T>().CountAsync(predicate, cancellationToken);
            _logger.LogDebug("Count {@Count}", count);
            return OperationResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to count entities");
            return OperationResult<int>.Failure("Unable to count entities", ex, RepositoryErrorCodes.CountEntitiesFailure);
        }
    }

    /// <summary>
    /// Deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the ID of the deleted entity.</returns>
    public virtual async Task<OperationResult<string>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.Set<T>().FindAsync([id], cancellationToken);
            if (entity == null) return OperationResult<string>.Failure("Entity not found.", RepositoryErrorCodes.EntityNotFound);

            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug(message: "Deleted entity {@Name}: {$Obj}", typeof(T).Name, entity.Id);
            return OperationResult<string>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete entity");
            return OperationResult<string>.Failure("Unable to delete entity", ex, RepositoryErrorCodes.DeleteEntityFailure);
        }
    }

    /// <summary>
    /// Deletes a range of entities by their IDs asynchronously.
    /// </summary>
    /// <param name="ids">The IDs of the entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the IDs of the deleted entities.</returns>
    public virtual async Task<OperationResult<IEnumerable<string>>> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entities = await context.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);

            if (entities.Count != 0)
            {
                context.Set<T>().RemoveRange(entities);
                await context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug(message: "Deleted entities {@Name}: {$Obj}", typeof(T).Name, ids);
                return OperationResult<IEnumerable<string>>.Success(ids);
            }
            _logger.LogDebug(message: "No entities found to be deleted {@Name}: {$Obj}", typeof(T).Name, 0);
            return OperationResult<IEnumerable<string>>.Failure("No entities found for deletion.", RepositoryErrorCodes.EntitiesNotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete entities");
            return OperationResult<IEnumerable<string>>.Failure("Unable to delete entities", ex, RepositoryErrorCodes.DeleteEntitiesFailure);
        }
    }

    /// <summary>
    /// Checks if any entities exist that satisfy the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including a boolean indicating if any entities exist.</returns>
    public virtual async Task<OperationResult<bool>> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var exists = await context.Set<T>().AnyAsync(predicate, cancellationToken);
            _logger.LogDebug("Exists {@Val}", exists);
            return OperationResult<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to check entity existance");
            return OperationResult<bool>.Failure("Unable to check entity existance", ex, RepositoryErrorCodes.CheckEntityExistsFailure);
        }
    }

    /// <summary>
    /// Finds entities that satisfy the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the matching entities.</returns>
    public virtual async Task<OperationResult<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entities = await context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
            if (entities.Count > 0)
                _logger.LogDebug(message: "Found {@Name}: {$Obj}", typeof(T).Name, entities.Select(e => e.Id));
            else
                _logger.LogDebug(message: "Not Found {@Name}: {$Obj}", typeof(T).Name, 0);
            return OperationResult<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to find entities");
            return OperationResult<IEnumerable<T>>.Failure("Unable to find entities", ex, RepositoryErrorCodes.FindEntitiesFailure);
        }
    }

    /// <summary>
    /// Finds entities with pagination that satisfy the specified predicate asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="order">The order by expression.</param>
    /// <param name="ascending">Indicates if the order should be ascending.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the paged list of entities.</returns>
    public virtual async Task<OperationResult<PagedList<T>>> FindAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? order = null, bool ascending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var pagedEntities = await context.Set<T>().GetPagedDataAsync(predicate, order, ascending, pageNumber, pageSize, cancellationToken);
            if (pagedEntities.Items.Any())
                _logger.LogDebug(message: "Found {@Name}: {$Obj}", typeof(T).Name, pagedEntities.Select(e => e.Id));
            else
                _logger.LogDebug(message: "Not Found {@Name}: {$Obj}", typeof(T).Name, 0);
            return OperationResult<PagedList<T>>.Success(pagedEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to find entities");
            return OperationResult<PagedList<T>>.Failure("Unable to find entities", ex, RepositoryErrorCodes.FindEntitiesFailure);
        }
    }

    /// <summary>
    /// Fetches all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the list of all entities.</returns>
    public virtual async Task<OperationResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entities = await context.Set<T>().ToListAsync(cancellationToken);
            if (entities.Count > 0)
                _logger.LogDebug(message: "Found {@Name}: {$Obj}", typeof(T).Name, entities.Select(e => e.Id));
            else
                _logger.LogDebug(message: "Not Found {@Name}: {$Obj}", typeof(T).Name, 0);
            return OperationResult<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to fetch all entities");
            return OperationResult<IEnumerable<T>>.Failure("Unable to fetch all entities", ex, RepositoryErrorCodes.FetchAllEntitiesFailure);
        }
    }

    /// <summary>
    /// Fetches all entities as <see cref="PagedList{T}"/> asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the paged list of all entities.</returns>
    public virtual async Task<OperationResult<PagedList<T>>> GetAllAsync(int pageNumber, int pageSize, Expression<Func<T, object>>? order = null, bool ascending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var pagedEntities = await context.Set<T>().GetPagedDataAsync(null, order, ascending, pageNumber, pageSize, cancellationToken);
            if (pagedEntities.Items.Any())
                _logger.LogDebug(message: "Found {@Name}: {$Obj}", typeof(T).Name, pagedEntities.Select(e => e.Id));
            else
                _logger.LogDebug(message: "Not Found {@Name}: {$Obj}", typeof(T).Name, 0);
            return OperationResult<PagedList<T>>.Success(pagedEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to fetch all entities");
            return OperationResult<PagedList<T>>.Failure("Unable to fetch all entities", ex, RepositoryErrorCodes.FetchAllEntitiesFailure);
        }
    }

    /// <summary>
    /// Fetches an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to fetch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the fetched entity.</returns>
    public virtual async Task<OperationResult<T>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.Set<T>().FindAsync([id], cancellationToken);
            if (entity is not null)
            {
                _logger.LogDebug(message: "Found {@Name}: {$Obj}", typeof(T).Name, entity.Id);
                return OperationResult<T>.Success(entity);
            }
            else
            {
                _logger.LogDebug(message: "Not Found {@Name}: {$Obj}", typeof(T).Name, 0);
                return OperationResult<T>.Failure("Entity not found.", RepositoryErrorCodes.EntityNotFound);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to fetch entity");
            return OperationResult<T>.Failure("Unable to fetch entity", ex, RepositoryErrorCodes.FetchEntityFailure);
        }
    }

    /// <summary>
    /// Soft deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to soft delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the ID of the soft deleted entity.</returns>
    public virtual async Task<OperationResult<string>> SoftDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.Set<T>().FindAsync([id], cancellationToken);
            if (entity == null) return OperationResult<string>.Failure("Entity not found.", RepositoryErrorCodes.EntityNotFound);

            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            if (isDeletedProperty != null)
            {
                isDeletedProperty.SetValue(entity, true);
                await context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug(message: "Soft Deleted {@Name}: {$Obj}", typeof(T).Name, entity.Id);
                return OperationResult<string>.Success(id);
            }
            _logger.LogDebug(message: "Unable to Soft Delete {@Name}: {$Obj}", typeof(T).Name, entity.Id);
            return OperationResult<string>.Failure("Soft delete not supported for this entity.", RepositoryErrorCodes.SoftDeleteNotSupported);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to soft delete entity");
            return OperationResult<string>.Failure("Unable to soft delete entity", ex, RepositoryErrorCodes.SoftDeleteEntityFailure);
        }
    }

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the ID of the updated entity.</returns>
    public virtual async Task<OperationResult<string>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            entity.LastModified = DateTime.UtcNow;
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug(message: "Updated entity {@Name}: {$Obj}", typeof(T).Name, entity.Id);
            return OperationResult<string>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update entity");
            return OperationResult<string>.Failure("Unable to update entity", ex, RepositoryErrorCodes.UpdateEntityFailure);
        }
    }

    /// <summary>
    /// Updates a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation, including the IDs of the updated entities.</returns>
    public virtual async Task<OperationResult<IEnumerable<string>>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            foreach (var entity in entities)
                entity.LastModified = DateTime.UtcNow;
            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            var ids = entities.Select(e => e.Id);
            _logger.LogDebug(message: "Updated entities {@Name}: {$Obj}", typeof(T).Name, ids);
            return OperationResult<IEnumerable<string>>.Success(ids);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update entities");
            return OperationResult<IEnumerable<string>>.Failure("Unable to update entities", ex, RepositoryErrorCodes.UpdateEntitiesFailure);
        }
    }
}