using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents an audit log entry for tracking changes to entities
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier for the audit log entry (auto-incrementing)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Name of the entity that was changed (Patient, Exam, User, etc.)
    /// </summary>
    [MaxLength(100)]
    public string EntityName { get; set; } = default!;
    
    /// <summary>
    /// ID of the specific entity instance that was changed (stored as string to accommodate GUIDs)
    /// </summary>
    public string EntityId { get; set; } = default!;
    
    /// <summary>
    /// Type of action performed (Insert, Update, Delete)
    /// </summary>
    [MaxLength(50)]
    public string Action { get; set; } = default!;
    
    /// <summary>
    /// UTC timestamp when the change occurred
    /// </summary>
    public DateTime TimestampUtc { get; set; }
    
    /// <summary>
    /// Username of the person who made the change
    /// </summary>
    [MaxLength(256)]
    public string? User { get; set; }
    
    /// <summary>
    /// JSON representation of the changes made (optional)
    /// </summary>
    public string? ChangesJson { get; set; }
}
