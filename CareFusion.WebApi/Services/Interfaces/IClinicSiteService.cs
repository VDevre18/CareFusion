using CareFusion.WebApi.DTOs;

namespace CareFusion.WebApi.Services.Interfaces;

public interface IClinicSiteService
{
    Task<IEnumerable<ClinicSiteDto>> GetClinicSitesAsync();
    Task<ClinicSiteDto?> GetClinicSiteByIdAsync(int id);
    Task<ClinicSiteDto> AddClinicSiteAsync(ClinicSiteDto dto);
    Task<ClinicSiteDto> UpdateClinicSiteAsync(ClinicSiteDto dto);
    Task<bool> DeleteClinicSiteAsync(int id);
    Task<ClinicSiteDto?> GetClinicSiteByCodeAsync(string code);
}