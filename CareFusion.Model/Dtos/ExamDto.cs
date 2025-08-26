// Placeholder for Dtos/ExamDto.cs
namespace CareFusion.Model.Dtos;

public record ExamDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string Modality { get; init; } = default!;
    public string StudyType { get; init; } = default!;
    public DateTime StudyDateUtc { get; init; }
    public string? StorageUri { get; init; }
    public string? Status { get; init; }
}
