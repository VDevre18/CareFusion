using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace CareFusion.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;

    public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Check if we're in a valid browser context
            var tokenResult = await _localStorage.GetAsync<string>("authToken");
            if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
            {
                // Get stored user information
                var usernameResult = await _localStorage.GetAsync<string>("username");
                var roleResult = await _localStorage.GetAsync<string>("userRole");
                var emailResult = await _localStorage.GetAsync<string>("userEmail");
                var clinicSiteResult = await _localStorage.GetAsync<string>("userClinicSiteId");

                var claims = new List<Claim>();
                
                if (usernameResult.Success && !string.IsNullOrEmpty(usernameResult.Value))
                    claims.Add(new Claim(ClaimTypes.Name, usernameResult.Value));
                
                if (emailResult.Success && !string.IsNullOrEmpty(emailResult.Value))
                    claims.Add(new Claim(ClaimTypes.Email, emailResult.Value));
                else if (usernameResult.Success && !string.IsNullOrEmpty(usernameResult.Value))
                    claims.Add(new Claim(ClaimTypes.Email, usernameResult.Value));
                
                if (roleResult.Success && !string.IsNullOrEmpty(roleResult.Value))
                    claims.Add(new Claim(ClaimTypes.Role, roleResult.Value));
                
                if (clinicSiteResult.Success && !string.IsNullOrEmpty(clinicSiteResult.Value))
                    claims.Add(new Claim("ClinicSiteId", clinicSiteResult.Value));

                var identity = new ClaimsIdentity(claims, "custom");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            // ProtectedLocalStorage might not be available during prerendering or initial load
        }
        
        // Return anonymous user by default
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task MarkUserAsAuthenticated(string username, string? role = null, string? email = null, int? clinicSiteId = null)
    {
        await _localStorage.SetAsync("authToken", "dummy-token");
        await _localStorage.SetAsync("username", username);
        if (!string.IsNullOrEmpty(role))
            await _localStorage.SetAsync("userRole", role);
        if (!string.IsNullOrEmpty(email))
            await _localStorage.SetAsync("userEmail", email);
        if (clinicSiteId.HasValue)
            await _localStorage.SetAsync("userClinicSiteId", clinicSiteId.Value.ToString());
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email ?? username),
        };
        
        if (!string.IsNullOrEmpty(role))
            claims.Add(new Claim(ClaimTypes.Role, role));
        
        if (clinicSiteId.HasValue)
            claims.Add(new Claim("ClinicSiteId", clinicSiteId.Value.ToString()));

        var identity = new ClaimsIdentity(claims, "custom");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.DeleteAsync("authToken");
        await _localStorage.DeleteAsync("username");
        await _localStorage.DeleteAsync("userRole");
        await _localStorage.DeleteAsync("userEmail");
        await _localStorage.DeleteAsync("userClinicSiteId");
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}