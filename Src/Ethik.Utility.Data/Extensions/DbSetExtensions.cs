using Ethik.Utility.Data.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="DbSet{TEntity}"/> in Entity Framework.
/// </summary>
public static class DbSetExtensions
{
    /// <summary>
    /// Retrieves a paginated list of data from the specified DbSet, with optional filtering, sorting, and pagination.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="dbSet">The DbSet to retrieve data from.</param>
    /// <param name="filter">An optional expression to filter the data.</param>
    /// <param name="order">An optional expression to order the data by.</param>
    /// <param name="ascending">Determines whether the data should be sorted in ascending or descending order. Default is true (ascending).</param>
    /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
    /// <param name="pageSize">The number of items per page. Default is 10.</param>
    /// <returns>A <see cref="PagedList{T}"/> containing the paginated data.</returns>
    public static async Task<PagedList<T>> GetPagedDataAsync<T>(
        this DbSet<T> dbSet,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) where T : class
    {
        // Initialize the query with AsNoTracking for performance.
        var query = dbSet.AsNoTracking().AsQueryable();

        // Apply filtering if a filter expression is provided.
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        // Apply sorting if an order expression is provided.
        if (order is not null)
        {
            query = ascending ? query.OrderBy(order) : query.OrderByDescending(order);
        }

        // Get the total item count for pagination.
        var totalItemCount = await query.CountAsync(cancellationToken);

        // Apply pagination by skipping and taking the appropriate number of items.
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Retrieve the paginated items.
        var items = await query.ToListAsync(cancellationToken);

        // Return the paginated list of items.
        return new PagedList<T>(items, totalItemCount, pageNumber, pageSize);
    }
}