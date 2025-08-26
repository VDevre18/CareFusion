using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace CareFusion.Web.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AuthenticationStateProvider _authStateProvider;
    public UserDto? CurrentUser { get; private set; }

    public AuthService(IHttpClientFactory httpFactory, AuthenticationStateProvider authStateProvider)
    {
        _httpFactory = httpFactory;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var resp = await http.PostAsJsonAsync("api/auth/login", new { username, password });
            if (!resp.IsSuccessStatusCode) return false;

            var api = await resp.Content.ReadFromJsonAsync<CareFusion.Model.Responses.ApiResponse<UserDto>>();
            if (api?.Success == true && api.Data != null)
            {
                CurrentUser = api.Data;
                
                // Mark user as authenticated in the state provider
                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    await customProvider.MarkUserAsAuthenticated(username);
                }
                
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        
        // Mark user as logged out in the state provider
        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            await customProvider.MarkUserAsLoggedOut();
        }
    }

    // Keep the old method for compatibility
    public void Logout() => _ = LogoutAsync();
}
