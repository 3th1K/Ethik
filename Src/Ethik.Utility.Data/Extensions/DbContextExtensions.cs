using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/> to provide additional functionalities such as auto-generating IDs with optional prefixes.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Sets an ID for the specified entity using a given property and an optional custom prefix.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity for which the ID is to be set.</param>
    /// <param name="idPropertyExpression">Expression to access the ID property of the entity.</param>
    /// <param name="customPrefix">Optional prefix to use for the generated ID. If null, a prefix is generated from the entity type name.</param>
    /// <exception cref="InvalidOperationException">Thrown if the expression is not a valid property accessor or if the property is not writable.</exception>
    private static void SetId<TEntity>(TEntity entity, Expression<Func<TEntity, object>> idPropertyExpression, string? customPrefix = null)
    {
        Type entityType = typeof(TEntity);

        // Resolve the member info (property) from the expression
        MemberExpression? memberExpression = GetMemberExpression(idPropertyExpression);
        if (memberExpression == null)
        {
            throw new InvalidOperationException($"Expression is not a valid property accessor for entity {entityType.Name}");
        }

        // Get the property info from the member expression
        var idProperty = entityType.GetProperty(memberExpression.Member.Name);
        if (idProperty != null && idProperty.CanWrite)
        {
            var prefix = customPrefix ?? GeneratePrefixFromTypeName(entityType.Name); // Use custom prefix or generate from type name
            var generatedId = GenerateIdWithPrefix(prefix);
            idProperty.SetValue(entity, generatedId);
        }
        else
        {
            throw new InvalidOperationException($"Entity {entityType.Name} does not have a writable Id property.");
        }
    }

    /// <summary>
    /// Extracts the <see cref="MemberExpression"/> from a given property expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="expression">The expression representing the property.</param>
    /// <returns>The <see cref="MemberExpression"/> or null if the expression is not valid.</returns>
    private static MemberExpression? GetMemberExpression<TEntity>(Expression<Func<TEntity, object>> expression)
    {
        return expression.Body switch
        {
            MemberExpression memberExpression => memberExpression,
            UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => memberExpression,
            _ => null
        };
    }

    /// <summary>
    /// Generates a prefix from the entity type name, by taking the first 3 characters of the type name and converting them to uppercase.
    /// </summary>
    /// <param name="typeName">The name of the entity type.</param>
    /// <returns>A string containing the first 3 uppercase characters of the type name.</returns>
    private static string GeneratePrefixFromTypeName(string typeName)
    {
        return new string(typeName.Take(3).ToArray()).ToUpper(); // Take first 3 characters and convert to uppercase
    }

    /// <summary>
    /// Generates an ID with the given prefix. The ID is a combination of the prefix and the current timestamp.
    /// </summary>
    /// <param name="prefix">The prefix to be used in the ID.</param>
    /// <returns>A string representing the generated ID.</returns>
    private static string GenerateIdWithPrefix(string prefix)
    {
        return $"{prefix}{DateTime.Now:yyMMddHHmmssff}";
    }

    /// <summary>
    /// Adds an entity to the <see cref="DbContext"/> with an auto-generated ID.
    /// The ID is generated based on the specified property and an optional custom prefix.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> in which the entity will be added.</param>
    /// <param name="entity">The entity to be added.</param>
    /// <param name="idPropertyExpression">Expression to access the ID property of the entity.</param>
    /// <param name="customPrefix">Optional custom prefix for the ID. If not provided, a prefix is generated from the entity type name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddEntityWithAutoIdAsync<TEntity>(
        this DbContext context,
        TEntity entity,
        Expression<Func<TEntity, object>> idPropertyExpression,
        string? customPrefix = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        SetId(entity, idPropertyExpression, customPrefix);
        await context.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Adds entities to the <see cref="DbContext"/> with an auto-generated ID.
    /// The ID is generated based on the specified property and an optional custom prefix.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> in which the entity will be added.</param>
    /// <param name="entities">The list of entities to be added.</param>
    /// <param name="idPropertyExpression">Expression to access the ID property of the entity.</param>
    /// <param name="customPrefix">Optional custom prefix for the ID. If not provided, a prefix is generated from the entity type name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AddEntitiesWithAutoIdAsync<TEntity>(
        this DbContext context,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, object>> idPropertyExpression,
        string? customPrefix = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        foreach (var entity in entities)
            SetId(entity, idPropertyExpression, customPrefix);
        await context.AddRangeAsync(entities, cancellationToken);
    }
}