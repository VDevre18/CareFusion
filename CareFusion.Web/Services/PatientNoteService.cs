using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class PatientNoteService : IPatientNoteService
{
    private readonly IHttpClientFactory _httpFactory;
    
    public PatientNoteService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<List<PatientNoteDto>> GetByPatientIdAsync(int patientId)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var url = $"api/patients/{patientId}/notes";
            Console.WriteLine($"Fetching notes from: {http.BaseAddress}{url}");
            
            var httpResponse = await http.GetAsync(url);
            Console.WriteLine($"Response status: {httpResponse.StatusCode}");
            
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<List<PatientNoteDto>>>();
                var notes = response?.Data ?? new List<PatientNoteDto>();
                Console.WriteLine($"Retrieved {notes.Count} notes for patient {patientId}");
                return notes;
            }
            else
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"API error: {httpResponse.StatusCode} - {errorContent}");
                return new List<PatientNoteDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patient notes: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<PatientNoteDto>();
        }
    }

    public async Task<PatientNoteDto?> GetAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<PatientNoteDto>>($"api/patient-notes/{id}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patient note: {ex.Message}");
            return null;
        }
    }

    public async Task<PatientNoteDto?> CreateAsync(PatientNoteDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            Console.WriteLine($"Creating note for patient {dto.PatientId}");
            Console.WriteLine($"Posting to: {http.BaseAddress}api/patient-notes");
            
            var response = await http.PostAsJsonAsync("api/patient-notes", dto);
            Console.WriteLine($"Create response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PatientNoteDto>>();
                var note = result?.Data;
                Console.WriteLine($"Successfully created note with ID: {note?.Id}");
                return note;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create API error: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating patient note: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<PatientNoteDto?> UpdateAsync(PatientNoteDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PutAsJsonAsync($"api/patient-notes/{dto.Id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PatientNoteDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient note: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.DeleteAsync($"api/patient-notes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient note: {ex.Message}");
            return false;
        }
    }
}