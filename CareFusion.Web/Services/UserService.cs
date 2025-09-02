using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

public class UserService : IUserService
{
    private readonly IHttpClientFactory _httpFactory;
    
    public UserService(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<PagedResult<UserDto>> SearchAsync(string? term, int page, int pageSize)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<PagedResult<UserDto>>>(
                $"api/users/search?term={Uri.EscapeDataString(term ?? "")}&page={page}&pageSize={pageSize}");
            return response?.Data ?? new PagedResult<UserDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching users: {ex.Message}");
            return new PagedResult<UserDto>();
        }
    }

    public async Task<UserDto?> GetAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<UserDto>>($"api/users/{id}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user: {ex.Message}");
            return null;
        }
    }

    public async Task<UserDto?> CreateAsync(UserDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PostAsJsonAsync("api/users", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return null;
        }
    }

    public async Task<UserDto?> UpdateAsync(UserDto dto)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.PutAsJsonAsync($"api/users/{dto.Id}", dto);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                return result?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return false;
        }
    }
}