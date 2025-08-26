using CareFusion.Model.Dtos;

namespace CareFusion.Web.Services.Interfaces;

public interface IAuthService
{
    UserDto? CurrentUser { get; }
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    void Logout();
}
