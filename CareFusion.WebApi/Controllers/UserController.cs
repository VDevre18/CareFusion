using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager _manager;

    public UsersController(UserManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Get(int id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll(CancellationToken ct)
    {
        var result = await _manager.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> Search(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _manager.SearchAsync(term, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetByUsername(string username, CancellationToken ct)
    {
        var result = await _manager.GetByUsernameAsync(username, ct);
        return Ok(result);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetByEmail(string email, CancellationToken ct)
    {
        var result = await _manager.GetByEmailAsync(email, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create(UserDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, UserDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _manager.UpdateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var result = await _manager.DeleteAsync(id, ct);
        return Ok(result);
    }
}