using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Web.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> SearchAsync(string? term, int page, int pageSize);
    Task<UserDto?> GetAsync(int id);
    Task<UserDto?> CreateAsync(UserDto dto);
    Task<UserDto?> UpdateAsync(UserDto dto);
    Task<bool> DeleteAsync(int id);
}