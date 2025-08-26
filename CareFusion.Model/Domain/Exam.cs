// Placeholder for Domain/Exam.cs
namespace CareFusion.Model.Domain;

public class Exam
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Modality { get; set; } = default!;
    public string StudyType { get; set; } = default!;
    public DateTime StudyDateUtc { get; set; }
    public string? StorageUri { get; set; }
    public string? Status { get; set; }
}
