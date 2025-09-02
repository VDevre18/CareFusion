using AutoMapper;
using CareFusion.Business.Services;
using CareFusion.WebApi.DTOs;
using CareFusion.WebApi.Services.Interfaces;

namespace CareFusion.WebApi.Services;

public class ClinicSiteService : IClinicSiteService
{
    private readonly ClinicSiteManager _clinicSiteManager;
    private readonly IMapper _mapper;

    public ClinicSiteService(ClinicSiteManager clinicSiteManager, IMapper mapper)
    {
        _clinicSiteManager = clinicSiteManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClinicSiteDto>> GetClinicSitesAsync()
    {
        var response = await _clinicSiteManager.GetAllAsync();
        return response.Success ? _mapper.Map<IEnumerable<ClinicSiteDto>>(response.Data) : [];
    }

    public async Task<ClinicSiteDto?> GetClinicSiteByIdAsync(int id)
    {
        var response = await _clinicSiteManager.GetByIdAsync(id);
        return response.Success ? _mapper.Map<ClinicSiteDto>(response.Data) : null;
    }

    public async Task<ClinicSiteDto> AddClinicSiteAsync(ClinicSiteDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.ClinicSiteDto>(dto);
        var response = await _clinicSiteManager.AddAsync(modelDto);
        return _mapper.Map<ClinicSiteDto>(response.Data);
    }

    public async Task<ClinicSiteDto> UpdateClinicSiteAsync(ClinicSiteDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.ClinicSiteDto>(dto);
        var response = await _clinicSiteManager.UpdateAsync(modelDto, "system");
        return _mapper.Map<ClinicSiteDto>(response.Data);
    }

    public async Task<bool> DeleteClinicSiteAsync(int id)
    {
        var response = await _clinicSiteManager.DeleteAsync(id);
        return response.Success;
    }

    public async Task<ClinicSiteDto?> GetClinicSiteByCodeAsync(string code)
    {
        var response = await _clinicSiteManager.GetByCodeAsync(code);
        return response.Success ? _mapper.Map<ClinicSiteDto>(response.Data) : null;
    }
}