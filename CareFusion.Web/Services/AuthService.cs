using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CareFusion.Web.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<AuthService> _logger;
    public UserDto? CurrentUser { get; private set; }

    public AuthService(IHttpClientFactory httpFactory, ILogger<AuthService> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            _logger.LogInformation("Attempting login for user: {Username}", username);
            var http = _httpFactory.CreateClient("Api");
            _logger.LogInformation("API Base URL: {BaseUrl}", http.BaseAddress);
            
            var loginRequest = new { username, password };
            var resp = await http.PostAsJsonAsync("api/auth/login", loginRequest);
            
            _logger.LogInformation("Login response status: {StatusCode}", resp.StatusCode);
            
            if (!resp.IsSuccessStatusCode) 
            {
                var errorContent = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("Login failed with status {StatusCode}. Response: {Response}", resp.StatusCode, errorContent);
                return false;
            }

            var api = await resp.Content.ReadFromJsonAsync<CareFusion.Model.Responses.ApiResponse<UserDto>>();
            if (api?.Success == true && api.Data != null)
            {
                CurrentUser = api.Data;
                _logger.LogInformation("Login successful for user: {Username}", username);
                return true;
            }
            
            _logger.LogWarning("Login failed - API response indicated failure: {Message}", api?.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login for user: {Username}", username);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
    }

    // Keep the old method for compatibility
    public void Logout() => _ = LogoutAsync();
}
