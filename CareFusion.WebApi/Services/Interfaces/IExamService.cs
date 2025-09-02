// Placeholder for Services/Interfaces/IExamService.cs
using CareFusion.WebApi.DTOs;

namespace CareFusion.WebApi.Services.Interfaces;

public interface IExamService
{
    Task<IEnumerable<ExamDto>> GetExamsByPatientIdAsync(int patientId);
    Task<ExamDto?> GetExamByIdAsync(int id);
    Task<ExamDto> AddExamAsync(ExamDto dto);
    Task<ExamDto> UpdateExamAsync(ExamDto dto);
    Task<bool> DeleteExamAsync(int id);
}
