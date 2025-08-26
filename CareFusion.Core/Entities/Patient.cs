// Placeholder for Entities/Patient.cs
using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

public class Patient : BaseEntity
{
    [MaxLength(100)]
    public string FirstName { get; set; } = default!;
    [MaxLength(100)]
    public string LastName { get; set; } = default!;
    [MaxLength(50)]
    public string? MRN { get; set; } // Medical Record Number (unique)
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(25)]
    public string? Gender { get; set; }

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
