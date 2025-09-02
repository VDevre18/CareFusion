namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for medical exam information
/// </summary>
public record ExamDto
{
    /// <summary>
    /// Unique identifier for the exam
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// ID of the patient this exam belongs to
    /// </summary>
    public int PatientId { get; init; }
    
    /// <summary>
    /// Type of imaging modality (CT, MRI, X-Ray, etc.)
    /// </summary>
    public string Modality { get; init; } = default!;
    
    /// <summary>
    /// Specific type of study being performed
    /// </summary>
    public string StudyType { get; init; } = default!;
    
    /// <summary>
    /// Date and time when the study was performed
    /// </summary>
    public DateTime StudyDateUtc { get; init; }
    
    /// <summary>
    /// URI where the exam images are stored
    /// </summary>
    public string? StorageUri { get; init; }
    
    /// <summary>
    /// Current status of the exam
    /// </summary>
    public string? Status { get; init; }
    
    /// <summary>
    /// Additional notes or comments about the exam
    /// </summary>
    public string? Notes { get; init; }
    
    /// <summary>
    /// Number of images associated with this exam
    /// </summary>
    public int ImageCount { get; init; }
    
    /// <summary>
    /// Type of exam (alias for StudyType)
    /// </summary>
    public string ExamType => StudyType;
    
    /// <summary>
    /// Description of the exam (alias for Notes)
    /// </summary>
    public string? Description => Notes;
    
    /// <summary>
    /// When the exam record was created
    /// </summary>
    public DateTime CreatedAt => StudyDateUtc;
    
    /// <summary>
    /// Indicates if the exam record is active
    /// </summary>
    public bool IsActive { get; init; } = true;
}
