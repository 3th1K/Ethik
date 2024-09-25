namespace Ethik.Utility.Data.Repository;

/// <summary>
/// Represents the base interface for an entity in the system.
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted.
    /// </summary>
    bool? IsDeleted { get; set; }
}