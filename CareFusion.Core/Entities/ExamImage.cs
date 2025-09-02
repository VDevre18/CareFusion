using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents an image file associated with a medical exam
/// </summary>
public class ExamImage : BaseEntity
{
    /// <summary>
    /// Foreign key to the Exam this image belongs to
    /// </summary>
    public int ExamId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated Exam
    /// </summary>
    public Exam Exam { get; set; } = default!;
    
    /// <summary>
    /// Original filename of the uploaded image
    /// </summary>
    [MaxLength(255)]
    public string FileName { get; set; } = default!;
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }
    
    /// <summary>
    /// MIME type of the image (image/jpeg, image/png, etc.)
    /// </summary>
    [MaxLength(100)]
    public string ContentType { get; set; } = default!;
    
    /// <summary>
    /// Path or URL where the image is stored
    /// </summary>
    [MaxLength(500)]
    public string FilePath { get; set; } = default!;
    
    /// <summary>
    /// Thumbnail image path for quick previews
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailPath { get; set; }
    
    /// <summary>
    /// Width of the image in pixels
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Height of the image in pixels
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Date when the image was uploaded
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Name of the person who uploaded the image
    /// </summary>
    [MaxLength(100)]
    public string? UploadedBy { get; set; }
}