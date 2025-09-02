// Placeholder for Services/PatientService.cs
using AutoMapper;
using CareFusion.Business.Services;
using CareFusion.WebApi.DTOs;
using CareFusion.WebApi.Services.Interfaces;

namespace CareFusion.WebApi.Services;

public class PatientService : IPatientService
{
    private readonly PatientManager _patientManager;
    private readonly IMapper _mapper;

    public PatientService(PatientManager patientManager, IMapper mapper)
    {
        _patientManager = patientManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PatientDto>> GetPatientsAsync()
    {
        var response = await _patientManager.GetAllAsync();
        return response.Success ? _mapper.Map<IEnumerable<PatientDto>>(response.Data) : [];
    }

    public async Task<PatientDto?> GetPatientByIdAsync(int id)
    {
        var response = await _patientManager.GetByIdAsync(id);
        return response.Success ? _mapper.Map<PatientDto>(response.Data) : null;
    }

    public async Task<PatientDto> AddPatientAsync(PatientDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.PatientDto>(dto);
        var response = await _patientManager.AddAsync(modelDto);
        return _mapper.Map<PatientDto>(response.Data);
    }

    public async Task<PatientDto> UpdatePatientAsync(PatientDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.PatientDto>(dto);
        var response = await _patientManager.UpdateAsync(modelDto, "system");
        return _mapper.Map<PatientDto>(response.Data);
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var response = await _patientManager.DeleteAsync(id);
        return response.Success;
    }
}
