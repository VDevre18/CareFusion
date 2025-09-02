using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IPatientNoteService
{
    Task<List<PatientNoteDto>> GetByPatientIdAsync(int patientId);
    Task<PatientNoteDto?> GetAsync(int id);
    Task<PatientNoteDto?> CreateAsync(PatientNoteDto dto);
    Task<PatientNoteDto?> UpdateAsync(PatientNoteDto dto);
    Task<bool> DeleteAsync(int id);
}