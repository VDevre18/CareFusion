// Placeholder for Services/ExamService.cs
using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class ExamService : IExamService
{
    private readonly IHttpClientFactory _httpFactory;
    public ExamService(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

    public async Task<PagedResult<ExamDto>> ListByPatientAsync(Guid patientId, int page, int pageSize)
    {
        var http = _httpFactory.CreateClient("Api");
        var res = await http.GetFromJsonAsync<ApiResponse<PagedResult<ExamDto>>>(
            $"api/exams/by-patient/{patientId}?page={page}&pageSize={pageSize}");
        return res?.Data ?? new();
    }

    public async Task<ExamDto?> GetAsync(Guid id)
    {
        var http = _httpFactory.CreateClient("Api");
        var res = await http.GetFromJsonAsync<ApiResponse<ExamDto>>($"api/exams/{id}");
        return res?.Data;
    }

    public async Task<ExamDto?> CreateAsync(ExamDto dto)
    {
        var http = _httpFactory.CreateClient("Api");
        var resp = await http.PostAsJsonAsync("api/exams", dto);
        var res = await resp.Content.ReadFromJsonAsync<ApiResponse<ExamDto>>();
        return res?.Data;
    }

    public async Task<ExamDto?> UpdateAsync(ExamDto dto)
    {
        var http = _httpFactory.CreateClient("Api");
        var resp = await http.PutAsJsonAsync($"api/exams/{dto.Id}", dto);
        var res = await resp.Content.ReadFromJsonAsync<ApiResponse<ExamDto>>();
        return res?.Data;
    }
}
