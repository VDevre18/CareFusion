// Placeholder for DTOs/PatientDto.cs
namespace CareFusion.WebApi.DTOs;

public class PatientDto
{
    public int Id { get; set; }
    public string MRN { get; set; } = string.Empty; // Medical Record Number
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
}
