namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for patient notes
/// </summary>
public record PatientNoteDto
{
    /// <summary>
    /// Unique identifier for the note
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// ID of the patient this note belongs to
    /// </summary>
    public int PatientId { get; init; }
    
    /// <summary>
    /// Type of note (Clinical Note, Observation, etc.)
    /// </summary>
    public string NoteType { get; init; } = "Clinical Note";
    
    /// <summary>
    /// The content/text of the note
    /// </summary>
    public string Content { get; init; } = default!;
    
    /// <summary>
    /// Name of the healthcare provider who created the note
    /// </summary>
    public string? AuthorName { get; init; }
    
    /// <summary>
    /// Date when the note was created
    /// </summary>
    public DateTime NoteDate { get; init; }
    
    /// <summary>
    /// Indicates if the note record is active
    /// </summary>
    public bool IsActive { get; init; } = true;
}