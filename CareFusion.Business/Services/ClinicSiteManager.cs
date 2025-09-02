using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Business.Validators;
using FluentValidation;

namespace CareFusion.Business.Services;

public class ClinicSiteManager
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<ClinicSiteDto> _validator;

    public ClinicSiteManager(IUnitOfWork uow, IMapper mapper, IValidator<ClinicSiteDto> validator)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
    }

    public virtual async Task<ApiResponse<ClinicSiteDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.ClinicSites.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<ClinicSiteDto>.Fail("Clinic site not found");

        return ApiResponse<ClinicSiteDto>.Ok(_mapper.Map<ClinicSiteDto>(entity));
    }

    public virtual async Task<ApiResponse<PagedResult<ClinicSiteDto>>> SearchAsync(
        string? term, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _uow.ClinicSites.SearchAsync(term, (page - 1) * pageSize, pageSize, ct);
        var result = new PagedResult<ClinicSiteDto>
        {
            Items = items.Select(_mapper.Map<ClinicSiteDto>).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return ApiResponse<PagedResult<ClinicSiteDto>>.Ok(result);
    }

    public virtual async Task<ApiResponse<ClinicSiteDto>> CreateAsync(ClinicSiteDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<ClinicSiteDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        // Check for duplicate code
        if (await _uow.ClinicSites.GetByCodeAsync(dto.Code, ct) != null)
            return ApiResponse<ClinicSiteDto>.Fail($"Clinic site with code '{dto.Code}' already exists");

        var entity = _mapper.Map<ClinicSite>(dto);
        await _uow.ClinicSites.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<ClinicSiteDto>.Ok(_mapper.Map<ClinicSiteDto>(entity), "Clinic site created successfully");
    }

    public virtual async Task<ApiResponse<ClinicSiteDto>> UpdateAsync(ClinicSiteDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<ClinicSiteDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = await _uow.ClinicSites.GetByIdAsync(dto.Id, ct);
        if (entity is null)
            return ApiResponse<ClinicSiteDto>.Fail("Clinic site not found");

        // Check for duplicate code (excluding current entity)
        var existingByCode = await _uow.ClinicSites.GetByCodeAsync(dto.Code, ct);
        if (existingByCode != null && existingByCode.Id != dto.Id)
            return ApiResponse<ClinicSiteDto>.Fail($"Clinic site with code '{dto.Code}' already exists");

        _mapper.Map(dto, entity);
        _uow.ClinicSites.Update(entity);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<ClinicSiteDto>.Ok(_mapper.Map<ClinicSiteDto>(entity), "Clinic site updated successfully");
    }

    public async Task<ApiResponse<List<ClinicSiteDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var clinicSites = await _uow.ClinicSites.GetAllAsync(ct);
        return ApiResponse<List<ClinicSiteDto>>.Ok(clinicSites.Select(_mapper.Map<ClinicSiteDto>).ToList());
    }

    public async Task<ApiResponse<ClinicSiteDto>> AddAsync(ClinicSiteDto dto, CancellationToken ct = default)
    {
        return await CreateAsync(dto, null, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.ClinicSites.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<bool>.Fail("Clinic site not found");

        _uow.ClinicSites.Remove(entity); // This does soft delete (sets IsActive = false)
        await _uow.SaveChangesAsync(null, ct);

        return ApiResponse<bool>.Ok(true, "Clinic site deleted successfully");
    }

    public async Task<ApiResponse<ClinicSiteDto>> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var entity = await _uow.ClinicSites.GetByCodeAsync(code, ct);
        if (entity is null)
            return ApiResponse<ClinicSiteDto>.Fail("Clinic site not found");

        return ApiResponse<ClinicSiteDto>.Ok(_mapper.Map<ClinicSiteDto>(entity));
    }
}