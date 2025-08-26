// Placeholder for Domain/Patient.cs
namespace CareFusion.Model.Domain;

public class Patient
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? MRN { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public List<Exam> Exams { get; set; } = new();
}
