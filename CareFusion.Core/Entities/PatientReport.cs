using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a medical report document for a patient
/// </summary>
public class PatientReport : BaseEntity
{
    /// <summary>
    /// Foreign key to the Patient this report belongs to
    /// </summary>
    public int PatientId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated Patient
    /// </summary>
    public Patient Patient { get; set; } = default!;
    
    /// <summary>
    /// Name of the report file
    /// </summary>
    [MaxLength(255)]
    public string FileName { get; set; } = default!;
    
    /// <summary>
    /// Type of report (Blood Test, Radiology, Lab Result, etc.)
    /// </summary>
    [MaxLength(100)]
    public string ReportType { get; set; } = default!;
    
    /// <summary>
    /// Description or title of the report
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }
    
    /// <summary>
    /// MIME type of the file (application/pdf, image/jpeg, etc.)
    /// </summary>
    [MaxLength(100)]
    public string ContentType { get; set; } = default!;
    
    /// <summary>
    /// Path or URL where the report file is stored
    /// </summary>
    [MaxLength(500)]
    public string FilePath { get; set; } = default!;
    
    /// <summary>
    /// Date when the report was uploaded
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Name of the healthcare provider who uploaded the report
    /// </summary>
    [MaxLength(100)]
    public string? UploadedBy { get; set; }
}