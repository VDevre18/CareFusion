// Placeholder for Services/ExamService.cs
using AutoMapper;
using CareFusion.Business.Services;
using CareFusion.WebApi.DTOs;
using CareFusion.WebApi.Services.Interfaces;

namespace CareFusion.WebApi.Services;

public class ExamService : IExamService
{
    private readonly ExamManager _examManager;
    private readonly IMapper _mapper;

    public ExamService(ExamManager examManager, IMapper mapper)
    {
        _examManager = examManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExamDto>> GetExamsByPatientIdAsync(int patientId)
    {
        var response = await _examManager.GetByPatientIdAsync(patientId);
        return response.Success ? _mapper.Map<IEnumerable<ExamDto>>(response.Data) : [];
    }

    public async Task<ExamDto?> GetExamByIdAsync(int id)
    {
        var response = await _examManager.GetByIdAsync(id);
        return response.Success ? _mapper.Map<ExamDto>(response.Data) : null;
    }

    public async Task<ExamDto> AddExamAsync(ExamDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.ExamDto>(dto);
        var response = await _examManager.AddAsync(modelDto);
        return _mapper.Map<ExamDto>(response.Data);
    }

    public async Task<ExamDto> UpdateExamAsync(ExamDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.ExamDto>(dto);
        var response = await _examManager.UpdateAsync(modelDto, "system");
        return _mapper.Map<ExamDto>(response.Data);
    }

    public async Task<bool> DeleteExamAsync(int id)
    {
        var response = await _examManager.DeleteAsync(id);
        return response.Success;
    }
}
