namespace CareFusion.Core.Entities;

/// <summary>
/// Base entity class that provides common properties for all entities
/// including ID, auditing fields, soft delete, and concurrency control
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key identifier for the entity (auto-incrementing integer)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was created
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
    
    /// <summary>
    /// Username of the person who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// UTC timestamp when the entity was last modified
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }
    
    /// <summary>
    /// Username of the person who last modified the entity
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Flag indicating if the entity has been soft deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public byte[]? RowVersion { get; set; }
}
