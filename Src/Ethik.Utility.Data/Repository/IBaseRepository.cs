using Ethik.Utility.Data.Collections;
using Ethik.Utility.Data.Results;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Repository;

public interface IBaseRepository<T> where T : class
{
    Task<OperationResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OperationResult<PagedList<T>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    Task<OperationResult<T>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<OperationResult<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<OperationResult<PagedList<T>>> FindAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? order = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    Task<OperationResult<string>> AddAsync(T entity, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default);

    Task<OperationResult<string>> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task<OperationResult<string>> DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<OperationResult<string>> SoftDeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<OperationResult<IEnumerable<string>>> AddRangeAsync(IEnumerable<T> entities, bool autoId = true, string? customPrefix = null, CancellationToken cancellationToken = default);

    Task<OperationResult<IEnumerable<string>>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<OperationResult<IEnumerable<string>>> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<OperationResult<bool>> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}