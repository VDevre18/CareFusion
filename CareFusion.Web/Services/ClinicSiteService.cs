using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class ClinicSiteService : IClinicSiteService
{
    private readonly IHttpClientFactory _httpFactory;
    
    public ClinicSiteService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<PagedResult<ClinicSiteDto>> SearchAsync(string? term, int page, int pageSize)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<PagedResult<ClinicSiteDto>>>(
                $"api/clinicsites/search?term={Uri.EscapeDataString(term ?? "")}&page={page}&pageSize={pageSize}");
            return response?.Data ?? new PagedResult<ClinicSiteDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching clinic sites: {ex.Message}");
            return new PagedResult<ClinicSiteDto>();
        }
    }

    public async Task<ClinicSiteDto?> GetAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<ClinicSiteDto>>($"api/clinicsites/{id}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting clinic site: {ex.Message}");
            return null;
        }
    }

    public async Task<ClinicSiteDto?> CreateAsync(ClinicSiteDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PostAsJsonAsync("api/clinicsites", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClinicSiteDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating clinic site: {ex.Message}");
            return null;
        }
    }

    public async Task<ClinicSiteDto?> UpdateAsync(ClinicSiteDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PutAsJsonAsync($"api/clinicsites/{dto.Id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClinicSiteDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating clinic site: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.DeleteAsync($"api/clinicsites/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting clinic site: {ex.Message}");
            return false;
        }
    }
}