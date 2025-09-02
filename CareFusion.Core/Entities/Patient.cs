using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a patient in the medical imaging system
/// </summary>
public class Patient : BaseEntity
{
    /// <summary>
    /// Patient's first name
    /// </summary>
    [MaxLength(100)]
    public string FirstName { get; set; } = default!;
    
    /// <summary>
    /// Patient's last name
    /// </summary>
    [MaxLength(100)]
    public string LastName { get; set; } = default!;
    
    /// <summary>
    /// Medical Record Number - unique identifier for the patient
    /// </summary>
    [MaxLength(50)]
    public string? MRN { get; set; }
    
    /// <summary>
    /// Patient's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Patient's gender (Male, Female, Other)
    /// </summary>
    [MaxLength(25)]
    public string? Gender { get; set; }
    
    /// <summary>
    /// Patient's phone number
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Patient's email address
    /// </summary>
    [MaxLength(256)]
    public string? Email { get; set; }
    
    /// <summary>
    /// Patient's address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Foreign key for the clinic site where this patient is primarily associated
    /// </summary>
    public int? ClinicSiteId { get; set; }
    
    /// <summary>
    /// Navigation property for the clinic site where this patient is primarily associated
    /// </summary>
    public ClinicSite? ClinicSite { get; set; }

    /// <summary>
    /// Navigation property for all exams associated with this patient
    /// </summary>
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    
    /// <summary>
    /// Navigation property for all notes associated with this patient
    /// </summary>
    public ICollection<PatientNote> Notes { get; set; } = new List<PatientNote>();
    
    /// <summary>
    /// Navigation property for all reports associated with this patient
    /// </summary>
    public ICollection<PatientReport> Reports { get; set; } = new List<PatientReport>();
}
