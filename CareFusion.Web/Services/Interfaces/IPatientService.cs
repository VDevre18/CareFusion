using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize, int? clinicSiteId = null);
    Task<PatientDto?> GetAsync(int id);
    Task<PatientDto?> CreateAsync(PatientDto dto);
    Task<PatientDto?> UpdateAsync(PatientDto dto);
    Task<List<PatientDto>> GetDeletedAsync();
    Task<List<PatientDto>> GetDuplicatesAsync();
    Task<PatientDto?> TransferPatientToClinicAsync(int patientId, int newClinicSiteId);
}
