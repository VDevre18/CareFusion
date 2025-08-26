// Placeholder for Entities/Exam.cs
using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

public class Exam : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    [MaxLength(100)]
    public string Modality { get; set; } = default!; // e.g., CT, MRI
    [MaxLength(100)]
    public string StudyType { get; set; } = default!;
    public DateTime StudyDateUtc { get; set; }

    // Storage metadata (blob path, etc.)
    [MaxLength(500)]
    public string? StorageUri { get; set; }
    [MaxLength(256)]
    public string? StorageKey { get; set; } // e.g., blob name

    // Simple status
    [MaxLength(50)]
    public string Status { get; set; } = "New";
}
