using AutoMapper;
using CareFusion.Business.Services;
using CareFusion.WebApi.DTOs;
using CareFusion.WebApi.Services.Interfaces;

namespace CareFusion.WebApi.Services;

public class UserService : IUserService
{
    private readonly UserManager _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var response = await _userManager.GetAllAsync();
        return response.Success ? _mapper.Map<IEnumerable<UserDto>>(response.Data) : [];
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var response = await _userManager.GetByIdAsync(id);
        return response.Success ? _mapper.Map<UserDto>(response.Data) : null;
    }

    public async Task<UserDto> AddUserAsync(UserDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.UserDto>(dto);
        var response = await _userManager.AddAsync(modelDto);
        return _mapper.Map<UserDto>(response.Data);
    }

    public async Task<UserDto> UpdateUserAsync(UserDto dto)
    {
        var modelDto = _mapper.Map<CareFusion.Model.Dtos.UserDto>(dto);
        var response = await _userManager.UpdateAsync(modelDto, "system");
        return _mapper.Map<UserDto>(response.Data);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _userManager.DeleteAsync(id);
        return response.Success;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var response = await _userManager.GetByUsernameAsync(username);
        return response.Success ? _mapper.Map<UserDto>(response.Data) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var response = await _userManager.GetByEmailAsync(email);
        return response.Success ? _mapper.Map<UserDto>(response.Data) : null;
    }
}