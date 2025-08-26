// Placeholder for Services/ExamManager.cs
using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Business.Validators;
using FluentValidation;

namespace CareFusion.Business.Services;

public class ExamManager
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<ExamDto> _validator;

    public ExamManager(IUnitOfWork uow, IMapper mapper, IValidator<ExamDto> validator)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
    }

    public virtual async Task<ApiResponse<ExamDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Exams.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<ExamDto>.Fail("Exam not found");

        return ApiResponse<ExamDto>.Ok(_mapper.Map<ExamDto>(entity));
    }

    public virtual async Task<ApiResponse<PagedResult<ExamDto>>> ListByPatientAsync(
        Guid patientId, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Exams.ListByPatientAsync(patientId, (page - 1) * pageSize, pageSize, ct);
        var result = new PagedResult<ExamDto>
        {
            Items = items.Select(_mapper.Map<ExamDto>).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return ApiResponse<PagedResult<ExamDto>>.Ok(result);
    }

    public virtual async Task<ApiResponse<ExamDto>> CreateAsync(ExamDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<ExamDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = _mapper.Map<Exam>(dto);
        await _uow.Exams.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<ExamDto>.Ok(_mapper.Map<ExamDto>(entity), "Exam created successfully");
    }

    public virtual async Task<ApiResponse<ExamDto>> UpdateAsync(ExamDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<ExamDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = await _uow.Exams.GetByIdAsync(dto.Id, ct);
        if (entity is null)
            return ApiResponse<ExamDto>.Fail("Exam not found");

        _mapper.Map(dto, entity);
        _uow.Exams.Update(entity);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<ExamDto>.Ok(_mapper.Map<ExamDto>(entity), "Exam updated successfully");
    }

    public async Task<ApiResponse<List<ExamDto>>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        var (exams, _) = await _uow.Exams.ListByPatientAsync(patientId, 0, int.MaxValue, ct);
        return ApiResponse<List<ExamDto>>.Ok(exams.Select(_mapper.Map<ExamDto>).ToList());
    }

    public async Task<ApiResponse<ExamDto>> AddAsync(ExamDto dto, CancellationToken ct = default)
    {
        return await CreateAsync(dto, null, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Exams.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<bool>.Fail("Exam not found");

        _uow.Exams.Remove(entity);
        await _uow.SaveChangesAsync(null, ct);

        return ApiResponse<bool>.Ok(true, "Exam deleted successfully");
    }
}
