// Placeholder for Services/Interfaces/IExamService.cs
using CareFusion.WebApi.DTOs;

namespace CareFusion.WebApi.Services.Interfaces;

public interface IExamService
{
    Task<IEnumerable<ExamDto>> GetExamsByPatientIdAsync(Guid patientId);
    Task<ExamDto?> GetExamByIdAsync(Guid id);
    Task<ExamDto> AddExamAsync(ExamDto dto);
    Task<ExamDto> UpdateExamAsync(ExamDto dto);
    Task<bool> DeleteExamAsync(Guid id);
}
