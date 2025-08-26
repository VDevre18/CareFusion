// Placeholder for Dtos/PatientDto.cs
namespace CareFusion.Model.Dtos;

public record PatientDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string? MRN { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }

    // Optional: summary info for UI
    public int ExamCount { get; init; }
}
