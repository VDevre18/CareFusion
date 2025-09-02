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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Get(int id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> Search(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? clinicSiteId = null,
        CancellationToken ct = default)
    {
        var result = await _manager.SearchAsync(term, page, pageSize, clinicSiteId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create(PatientDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(int id, PatientDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _manager.UpdateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<ApiResponse<List<PatientDto>>>> GetDeleted(CancellationToken ct = default)
    {
        var result = await _manager.GetDeletedAsync(ct);
        return Ok(result);
    }

    [HttpGet("duplicates")]
    public async Task<ActionResult<ApiResponse<List<PatientDto>>>> GetDuplicates(CancellationToken ct = default)
    {
        var result = await _manager.GetDuplicatesAsync(ct);
        return Ok(result);
    }

    [HttpPut("{id:int}/transfer/{newClinicSiteId:int}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> TransferToClinic(int id, int newClinicSiteId, CancellationToken ct = default)
    {
        var result = await _manager.TransferPatientToClinicAsync(id, newClinicSiteId, User.Identity?.Name, ct);
        return Ok(result);
    }
}
