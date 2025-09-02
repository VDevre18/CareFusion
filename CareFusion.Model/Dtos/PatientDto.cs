namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for patient information
/// </summary>
public class PatientDto
{
    /// <summary>
    /// Unique identifier for the patient (sequential integer like PT-001)
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Patient's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Patient's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Medical Record Number
    /// </summary>
    public string? MRN { get; set; }
    
    /// <summary>
    /// Patient's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Patient's gender
    /// </summary>
    public string? Gender { get; set; }
    
    /// <summary>
    /// Patient's phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Patient's email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Patient's address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// ID of the clinic site where this patient is primarily associated
    /// </summary>
    public int? ClinicSiteId { get; set; }
    
    /// <summary>
    /// Name of the clinic site where this patient is primarily associated
    /// </summary>
    public string? ClinicSiteName { get; set; }

    /// <summary>
    /// Total number of exams for this patient
    /// </summary>
    public int ExamCount { get; set; }
    
    /// <summary>
    /// Date of the last visit
    /// </summary>
    public DateTime? LastVisit { get; set; }

    /// <summary>
    /// Full name (FirstName + LastName)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Indicates if the patient record is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the patient record was created
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
    
    /// <summary>
    /// When the patient record was last modified
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }
    
    /// <summary>
    /// Who last modified the patient record
    /// </summary>
    public string? ModifiedBy { get; set; }
    
    /// <summary>
    /// Indicates if the patient record is deleted (soft delete)
    /// </summary>
    public bool IsDeleted { get; set; }
}
