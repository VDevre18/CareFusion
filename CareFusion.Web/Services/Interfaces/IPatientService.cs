using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize);
    Task<PatientDto?> GetAsync(Guid id);
    Task<PatientDto?> CreateAsync(PatientDto dto);
    Task<PatientDto?> UpdateAsync(PatientDto dto);
}
