// Placeholder for DTOs/ExamDto.cs
namespace CareFusion.WebApi.DTOs;

public class ExamDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReportUrl { get; set; }
}
