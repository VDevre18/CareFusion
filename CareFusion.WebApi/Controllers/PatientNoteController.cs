using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/patient-notes")]
public class PatientNoteController : ControllerBase
{
    private readonly PatientNoteManager _manager;

    public PatientNoteController(PatientNoteManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientNoteDto>>> Get(int id, CancellationToken ct)
    {
        var result = await _manager.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("by-patient/{patientId:int}")]
    public async Task<ActionResult<ApiResponse<List<PatientNoteDto>>>> GetByPatient(int patientId, CancellationToken ct)
    {
        var result = await _manager.GetByPatientIdAsync(patientId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientNoteDto>>> Create(PatientNoteDto dto, CancellationToken ct)
    {
        var result = await _manager.CreateAsync(dto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientNoteDto>>> Update(int id, PatientNoteDto dto, CancellationToken ct)
    {
        var updatedDto = dto with { Id = id };
        var result = await _manager.UpdateAsync(updatedDto, User.Identity?.Name, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var result = await _manager.DeleteAsync(id, ct);
        return Ok(result);
    }
}

[ApiController]
[Route("api/patients/{patientId:int}/notes")]
public class PatientNotesController : ControllerBase
{
    private readonly PatientNoteManager _manager;

    public PatientNotesController(PatientNoteManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PatientNoteDto>>>> GetByPatient(int patientId, CancellationToken ct)
    {
        var result = await _manager.GetByPatientIdAsync(patientId, ct);
        return Ok(result);
    }
}