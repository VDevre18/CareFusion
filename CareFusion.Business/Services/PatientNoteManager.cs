using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Business.Services;

public class PatientNoteManager
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PatientNoteManager(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public virtual async Task<ApiResponse<PatientNoteDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.PatientNotes.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<PatientNoteDto>.Fail("Patient note not found");

        return ApiResponse<PatientNoteDto>.Ok(_mapper.Map<PatientNoteDto>(entity));
    }

    public virtual async Task<ApiResponse<List<PatientNoteDto>>> GetByPatientIdAsync(int patientId, CancellationToken ct = default)
    {
        var entities = await _uow.PatientNotes.GetByPatientIdAsync(patientId, ct);
        var dtos = entities.Select(_mapper.Map<PatientNoteDto>).ToList();
        return ApiResponse<List<PatientNoteDto>>.Ok(dtos);
    }

    public virtual async Task<ApiResponse<PatientNoteDto>> CreateAsync(PatientNoteDto dto, string? user, CancellationToken ct = default)
    {
        // Validate that patient exists
        var patient = await _uow.Patients.GetByIdAsync(dto.PatientId, ct);
        if (patient is null)
            return ApiResponse<PatientNoteDto>.Fail("Patient not found");

        var entity = _mapper.Map<PatientNote>(dto);
        entity.CreatedAtUtc = DateTime.UtcNow;
        entity.CreatedBy = user ?? "System";

        await _uow.PatientNotes.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<PatientNoteDto>.Ok(_mapper.Map<PatientNoteDto>(entity), "Patient note created successfully");
    }

    public virtual async Task<ApiResponse<PatientNoteDto>> UpdateAsync(PatientNoteDto dto, string? user, CancellationToken ct = default)
    {
        var entity = await _uow.PatientNotes.GetByIdAsync(dto.Id, ct);
        if (entity is null)
            return ApiResponse<PatientNoteDto>.Fail("Patient note not found");

        _mapper.Map(dto, entity);
        entity.ModifiedAtUtc = DateTime.UtcNow;
        entity.ModifiedBy = user ?? "System";

        _uow.PatientNotes.Update(entity);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<PatientNoteDto>.Ok(_mapper.Map<PatientNoteDto>(entity), "Patient note updated successfully");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.PatientNotes.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<bool>.Fail("Patient note not found");

        _uow.PatientNotes.Remove(entity); // This does soft delete (sets IsActive = false)
        await _uow.SaveChangesAsync(null, ct);

        return ApiResponse<bool>.Ok(true, "Patient note deleted successfully");
    }
}