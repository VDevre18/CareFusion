using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a clinical note or observation for a patient
/// </summary>
public class PatientNote : BaseEntity
{
    /// <summary>
    /// Foreign key to the Patient this note belongs to
    /// </summary>
    public int PatientId { get; set; }
    
    /// <summary>
    /// Navigation property to the associated Patient
    /// </summary>
    public Patient Patient { get; set; } = default!;
    
    /// <summary>
    /// Type of note (Clinical Note, Observation, etc.)
    /// </summary>
    [MaxLength(50)]
    public string NoteType { get; set; } = "Clinical Note";
    
    /// <summary>
    /// The content/text of the note
    /// </summary>
    [MaxLength(2000)]
    public string Content { get; set; } = default!;
    
    /// <summary>
    /// Name of the healthcare provider who created the note
    /// </summary>
    [MaxLength(100)]
    public string? AuthorName { get; set; }
    
    /// <summary>
    /// Date when the note was created
    /// </summary>
    public DateTime NoteDate { get; set; } = DateTime.UtcNow;
}