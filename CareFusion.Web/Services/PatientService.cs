// Placeholder for Services/PatientService.cs
using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class PatientService : IPatientService
{
    private readonly IHttpClientFactory _httpFactory;
    public PatientService(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

    public async Task<PagedResult<PatientDto>> SearchAsync(string? term, int page, int pageSize)
    {
        var http = _httpFactory.CreateClient("Api");
        var res = await http.GetFromJsonAsync<ApiResponse<PagedResult<PatientDto>>>(
            $"api/patients/search?term={Uri.EscapeDataString(term ?? "")}&page={page}&pageSize={pageSize}");
        return res?.Data ?? new();
    }

    public async Task<PatientDto?> GetAsync(Guid id)
    {
        var http = _httpFactory.CreateClient("Api");
        var res = await http.GetFromJsonAsync<ApiResponse<PatientDto>>($"api/patients/{id}");
        return res?.Data;
    }

    public async Task<PatientDto?> CreateAsync(PatientDto dto)
    {
        var http = _httpFactory.CreateClient("Api");
        var resp = await http.PostAsJsonAsync("api/patients", dto);
        var res = await resp.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        return res?.Data;
    }

    public async Task<PatientDto?> UpdateAsync(PatientDto dto)
    {
        var http = _httpFactory.CreateClient("Api");
        var resp = await http.PutAsJsonAsync($"api/patients/{dto.Id}", dto);
        var res = await resp.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        return res?.Data;
    }
}
