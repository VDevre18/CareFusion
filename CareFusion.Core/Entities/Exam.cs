using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a medical imaging exam for a patient
/// </summary>
public class Exam : BaseEntity
{
    /// <summary>
    /// Foreign key to the Patient this exam belongs to
    /// </summary>
    public int PatientId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated Patient
    /// </summary>
    public Patient Patient { get; set; } = default!;

    /// <summary>
    /// Type of imaging modality (CT, MRI, X-Ray, etc.)
    /// </summary>
    [MaxLength(100)]
    public string Modality { get; set; } = default!;
    
    /// <summary>
    /// Specific type of study being performed
    /// </summary>
    [MaxLength(100)]
    public string StudyType { get; set; } = default!;
    
    /// <summary>
    /// Date and time when the study was performed (UTC)
    /// </summary>
    public DateTime StudyDateUtc { get; set; }

    /// <summary>
    /// URI where the exam images are stored in cloud storage
    /// </summary>
    [MaxLength(500)]
    public string? StorageUri { get; set; }
    
    /// <summary>
    /// Unique key for identifying the exam in storage systems
    /// </summary>
    [MaxLength(256)]
    public string? StorageKey { get; set; }

    /// <summary>
    /// Current status of the exam (New, In Progress, Completed, etc.)
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = "New";
    
    /// <summary>
    /// Additional notes or comments about the exam
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Navigation property for all images associated with this exam
    /// </summary>
    public ICollection<ExamImage> Images { get; set; } = new List<ExamImage>();
}
