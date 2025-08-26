// Placeholder for Controllers/PatientController.cs
using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientManager _manager;

    public PatientsController(PatientManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Get(Guid id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> Search(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _manager.SearchAsync(term, page, pageSize, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create(PatientDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(Guid id, PatientDto dto, CancellationToken ct)
    {
        dto = dto with { Id = id };
        var result = await _manager.UpdateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }
}
