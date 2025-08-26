// Placeholder for Controllers/ExamController.cs
using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly ExamManager _manager;

    public ExamsController(ExamManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExamDto>>> Get(Guid id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<ApiResponse<PagedResult<ExamDto>>>> ListByPatient(
        Guid patientId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _manager.ListByPatientAsync(patientId, page, pageSize, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ExamDto>>> Create(ExamDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExamDto>>> Update(Guid id, ExamDto dto, CancellationToken ct)
    {
        dto = dto with { Id = id };
        var result = await _manager.UpdateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }
}
