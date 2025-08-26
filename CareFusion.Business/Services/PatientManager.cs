// Placeholder for Services/PatientManager.cs
using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Business.Validators;
using FluentValidation;

namespace CareFusion.Business.Services;

public class PatientManager
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<PatientDto> _validator;

    public PatientManager(IUnitOfWork uow, IMapper mapper, IValidator<PatientDto> validator)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
    }

    public virtual async Task<ApiResponse<PatientDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Patients.GetWithExamsAsync(id, ct);
        if (entity is null)
            return ApiResponse<PatientDto>.Fail("Patient not found");

        return ApiResponse<PatientDto>.Ok(_mapper.Map<PatientDto>(entity));
    }

    public virtual async Task<ApiResponse<PagedResult<PatientDto>>> SearchAsync(
        string? term, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Patients.SearchAsync(term, (page - 1) * pageSize, pageSize, ct);
        var result = new PagedResult<PatientDto>
        {
            Items = items.Select(_mapper.Map<PatientDto>).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return ApiResponse<PagedResult<PatientDto>>.Ok(result);
    }

    public virtual async Task<ApiResponse<PatientDto>> CreateAsync(PatientDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<PatientDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        if (!string.IsNullOrWhiteSpace(dto.MRN) && await _uow.Patients.ExistsByMrnAsync(dto.MRN!, ct))
            return ApiResponse<PatientDto>.Fail($"Patient with MRN {dto.MRN} already exists");

        var entity = _mapper.Map<Patient>(dto);
        await _uow.Patients.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<PatientDto>.Ok(_mapper.Map<PatientDto>(entity), "Patient created successfully");
    }

    public virtual async Task<ApiResponse<PatientDto>> UpdateAsync(PatientDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<PatientDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = await _uow.Patients.GetByIdAsync(dto.Id, ct);
        if (entity is null)
            return ApiResponse<PatientDto>.Fail("Patient not found");

        _mapper.Map(dto, entity);
        _uow.Patients.Update(entity);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<PatientDto>.Ok(_mapper.Map<PatientDto>(entity), "Patient updated successfully");
    }

    public async Task<ApiResponse<List<PatientDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var patients = await _uow.Patients.GetAllAsync(ct);
        return ApiResponse<List<PatientDto>>.Ok(patients.Select(_mapper.Map<PatientDto>).ToList());
    }

    public async Task<ApiResponse<PatientDto>> AddAsync(PatientDto dto, CancellationToken ct = default)
    {
        return await CreateAsync(dto, null, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Patients.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<bool>.Fail("Patient not found");

        _uow.Patients.Remove(entity);
        await _uow.SaveChangesAsync(null, ct);

        return ApiResponse<bool>.Ok(true, "Patient deleted successfully");
    }
}
