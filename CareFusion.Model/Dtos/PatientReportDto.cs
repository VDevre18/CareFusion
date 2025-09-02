namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for patient reports
/// </summary>
public record PatientReportDto
{
    /// <summary>
    /// Unique identifier for the report
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// ID of the patient this report belongs to
    /// </summary>
    public int PatientId { get; init; }
    
    /// <summary>
    /// Name of the report file
    /// </summary>
    public string FileName { get; init; } = default!;
    
    /// <summary>
    /// Type of report (Blood Test, Radiology, Lab Result, etc.)
    /// </summary>
    public string ReportType { get; init; } = default!;
    
    /// <summary>
    /// Description or title of the report
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }
    
    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string ContentType { get; init; } = default!;
    
    /// <summary>
    /// Path or URL where the report file is stored
    /// </summary>
    public string FilePath { get; init; } = default!;
    
    /// <summary>
    /// Date when the report was uploaded
    /// </summary>
    public DateTime UploadDate { get; init; }
    
    /// <summary>
    /// Name of the person who uploaded the report
    /// </summary>
    public string? UploadedBy { get; init; }
    
    /// <summary>
    /// Human-readable file size (e.g., "2.4 MB")
    /// </summary>
    public string FileSizeFormatted { get; init; } = default!;
    
    /// <summary>
    /// When the report record was created
    /// </summary>
    public DateTime CreatedAt => UploadDate;
    
    /// <summary>
    /// Indicates if the report record is active
    /// </summary>
    public bool IsActive { get; init; } = true;
}