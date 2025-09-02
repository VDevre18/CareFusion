using CareFusion.WebApi.DTOs;

namespace CareFusion.WebApi.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> AddUserAsync(UserDto dto);
    Task<UserDto> UpdateUserAsync(UserDto dto);
    Task<bool> DeleteUserAsync(int id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto?> GetUserByEmailAsync(string email);
}