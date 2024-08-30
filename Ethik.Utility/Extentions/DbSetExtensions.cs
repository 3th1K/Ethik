using Ethik.Utility.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ethik.Utility.Extensions;

public static class DbSetExtensions
{
    public static async Task<PagedList<T>> GetPaginatedDataAsync<T>(
        this DbSet<T> dbSet,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        int pageNumber = 1,
        int pageSize = 10) where T : class
    {
        var query = dbSet.AsNoTracking().AsQueryable();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (order is not null)
        {
            if (ascending)
            {
                query = query.OrderBy(order);
            }
            else
            {
                query = query.OrderByDescending(order);
            }
        }

        var totalItemCount = await query.CountAsync();
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var items = await query.ToListAsync();
        return new PagedList<T>(items, totalItemCount, pageNumber, pageSize);
    }
}
