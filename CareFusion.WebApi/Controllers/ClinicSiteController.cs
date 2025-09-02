using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicSitesController : ControllerBase
{
    private readonly ClinicSiteManager _manager;

    public ClinicSitesController(ClinicSiteManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ClinicSiteDto>>> Get(int id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ClinicSiteDto>>>> GetAll(CancellationToken ct)
    {
        var result = await _manager.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ClinicSiteDto>>>> Search(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _manager.SearchAsync(term, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<ApiResponse<ClinicSiteDto>>> GetByCode(string code, CancellationToken ct)
    {
        var result = await _manager.GetByCodeAsync(code, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClinicSiteDto>>> Create(ClinicSiteDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ClinicSiteDto>>> Update(int id, ClinicSiteDto dto, CancellationToken ct)
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