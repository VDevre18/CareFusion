// Placeholder for Services/Interfaces/IPatientService.cs
using CareFusion.WebApi.DTOs;

namespace CareFusion.WebApi.Services.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetPatientsAsync();
    Task<PatientDto?> GetPatientByIdAsync(Guid id);
    Task<PatientDto> AddPatientAsync(PatientDto dto);
    Task<PatientDto> UpdatePatientAsync(PatientDto dto);
    Task<bool> DeletePatientAsync(Guid id);
}
