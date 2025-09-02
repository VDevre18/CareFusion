using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IClinicSiteService
{
    Task<PagedResult<ClinicSiteDto>> SearchAsync(string? term, int page, int pageSize);
    Task<ClinicSiteDto?> GetAsync(int id);
    Task<ClinicSiteDto?> CreateAsync(ClinicSiteDto dto);
    Task<ClinicSiteDto?> UpdateAsync(ClinicSiteDto dto);
    Task<bool> DeleteAsync(int id);
}