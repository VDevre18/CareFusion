// Placeholder for Entities/AuditLog.cs
using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

public class AuditLog
{
    public long Id { get; set; } // big identity
    [MaxLength(100)]
    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }
    [MaxLength(50)]
    public string Action { get; set; } = default!; // Insert/Update/Delete
    public DateTime TimestampUtc { get; set; }
    [MaxLength(256)]
    public string? User { get; set; }
    public string? ChangesJson { get; set; } // optional diff payload
}
