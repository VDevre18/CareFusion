namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for exam images
/// </summary>
public record ExamImageDto
{
    /// <summary>
    /// Unique identifier for the image
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// ID of the exam this image belongs to
    /// </summary>
    public int ExamId { get; init; }
    
    /// <summary>
    /// Original filename of the uploaded image
    /// </summary>
    public string FileName { get; init; } = default!;
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }
    
    /// <summary>
    /// MIME type of the image
    /// </summary>
    public string ContentType { get; init; } = default!;
    
    /// <summary>
    /// Path or URL where the image is stored
    /// </summary>
    public string FilePath { get; init; } = default!;
    
    /// <summary>
    /// Thumbnail image path for quick previews
    /// </summary>
    public string? ThumbnailPath { get; init; }
    
    /// <summary>
    /// Width of the image in pixels
    /// </summary>
    public int? Width { get; init; }
    
    /// <summary>
    /// Height of the image in pixels
    /// </summary>
    public int? Height { get; init; }
    
    /// <summary>
    /// Date when the image was uploaded
    /// </summary>
    public DateTime UploadDate { get; init; }
    
    /// <summary>
    /// Name of the person who uploaded the image
    /// </summary>
    public string? UploadedBy { get; init; }
    
    /// <summary>
    /// Human-readable file size (e.g., "1.2 MB")
    /// </summary>
    public string FileSizeFormatted { get; init; } = default!;
    
    /// <summary>
    /// When the image record was created
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
    
    /// <summary>
    /// When the image record was last modified
    /// </summary>
    public DateTime? ModifiedAtUtc { get; init; }
    
    /// <summary>
    /// Who last modified the image record
    /// </summary>
    public string? ModifiedBy { get; init; }
    
    /// <summary>
    /// Indicates if the image record is deleted (soft delete)
    /// </summary>
    public bool IsDeleted { get; init; }
    
    /// <summary>
    /// Patient name for this image (from exam relationship)
    /// </summary>
    public string? PatientName { get; init; }
    
    /// <summary>
    /// Exam date for this image (from exam relationship)
    /// </summary>
    public DateTime? ExamDate { get; init; }
}