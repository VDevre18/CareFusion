// Placeholder for Controllers/AuthController.cs
using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthManager _manager;

    public AuthController(AuthManager manager)
    {
        _manager = manager;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await _manager.LoginAsync(request.Username, request.Password, ct);
        return Ok(result);
    }
}

public record LoginRequest(string Username, string Password);
