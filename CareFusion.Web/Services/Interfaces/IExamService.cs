using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IExamService
{
    Task<PagedResult<ExamDto>> ListByPatientAsync(Guid patientId, int page, int pageSize);
    Task<ExamDto?> GetAsync(Guid id);
    Task<ExamDto?> CreateAsync(ExamDto dto);
    Task<ExamDto?> UpdateAsync(ExamDto dto);
}
