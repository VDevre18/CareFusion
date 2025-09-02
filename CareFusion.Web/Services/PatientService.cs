using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class PatientService : IPatientService
{
    private readonly IHttpClientFactory _httpFactory;
    
    public PatientService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize, int? clinicSiteId = null)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var queryParams = $"term={Uri.EscapeDataString(term ?? "")}&page={page}&pageSize={pageSize}";
            if (clinicSiteId.HasValue)
                queryParams += $"&clinicSiteId={clinicSiteId.Value}";
            
            var response = await http.GetFromJsonAsync<ApiResponse<PagedResult<PatientDto>>>(
                $"api/patients/search?{queryParams}");
            return response?.Data ?? new PagedResult<PatientDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching patients: {ex.Message}");
            return new PagedResult<PatientDto>();
        }
    }

    public async Task<PatientDto?> GetAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<PatientDto>>($"api/patients/{id}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patient: {ex.Message}");
            return null;
        }
    }

    public async Task<PatientDto?> CreateAsync(PatientDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PostAsJsonAsync("api/patients", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating patient: {ex.Message}");
            return null;
        }
    }

    public async Task<PatientDto?> UpdateAsync(PatientDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PutAsJsonAsync($"api/patients/{dto.Id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient: {ex.Message}");
            return null;
        }
    }

    public async Task<List<PatientDto>> GetDeletedAsync()
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<List<PatientDto>>>("api/patients/deleted");
            return response?.Data ?? new List<PatientDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting deleted patients: {ex.Message}");
            return new List<PatientDto>();
        }
    }

    public async Task<List<PatientDto>> GetDuplicatesAsync()
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<List<PatientDto>>>("api/patients/duplicates");
            return response?.Data ?? new List<PatientDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting duplicate patients: {ex.Message}");
            return new List<PatientDto>();
        }
    }

    public async Task<PatientDto?> TransferPatientToClinicAsync(int patientId, int newClinicSiteId)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PutAsync($"api/patients/{patientId}/transfer/{newClinicSiteId}", null);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error transferring patient: {ex.Message}");
            return null;
        }
    }
}
