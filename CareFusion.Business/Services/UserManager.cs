using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Business.Validators;
using FluentValidation;

namespace CareFusion.Business.Services;

public class UserManager
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<UserDto> _validator;

    public UserManager(IUnitOfWork uow, IMapper mapper, IValidator<UserDto> validator)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
    }

    public virtual async Task<ApiResponse<UserDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Users.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<UserDto>.Fail("User not found");

        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(entity));
    }

    public virtual async Task<ApiResponse<PagedResult<UserDto>>> SearchAsync(
        string? term, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Users.SearchAsync(term, (page - 1) * pageSize, pageSize, ct);
        var result = new PagedResult<UserDto>
        {
            Items = items.Select(_mapper.Map<UserDto>).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
        return ApiResponse<PagedResult<UserDto>>.Ok(result);
    }

    public virtual async Task<ApiResponse<UserDto>> CreateAsync(UserDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<UserDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        // Check for duplicate username
        if (await _uow.Users.GetByUsernameAsync(dto.Username, ct) != null)
            return ApiResponse<UserDto>.Fail($"User with username '{dto.Username}' already exists");

        // Check for duplicate email
        if (await _uow.Users.GetByEmailAsync(dto.Email, ct) != null)
            return ApiResponse<UserDto>.Fail($"User with email '{dto.Email}' already exists");

        var entity = _mapper.Map<User>(dto);
        
        // Hash the provided password
        if (!string.IsNullOrEmpty(dto.Password))
        {
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }
        else
        {
            return ApiResponse<UserDto>.Fail("Password is required for new users");
        }

        await _uow.Users.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(entity), "User created successfully");
    }

    public virtual async Task<ApiResponse<UserDto>> UpdateAsync(UserDto dto, string? user, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return ApiResponse<UserDto>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = await _uow.Users.GetByIdAsync(dto.Id, ct);
        if (entity is null)
            return ApiResponse<UserDto>.Fail("User not found");

        // Check for duplicate username (excluding current user)
        var existingByUsername = await _uow.Users.GetByUsernameAsync(dto.Username, ct);
        if (existingByUsername != null && existingByUsername.Id != dto.Id)
            return ApiResponse<UserDto>.Fail($"User with username '{dto.Username}' already exists");

        // Check for duplicate email (excluding current user)
        var existingByEmail = await _uow.Users.GetByEmailAsync(dto.Email, ct);
        if (existingByEmail != null && existingByEmail.Id != dto.Id)
            return ApiResponse<UserDto>.Fail($"User with email '{dto.Email}' already exists");

        // Map properties but keep the original password hash
        var originalPasswordHash = entity.PasswordHash;
        _mapper.Map(dto, entity);
        entity.PasswordHash = originalPasswordHash; // Don't change password through regular update

        _uow.Users.Update(entity);
        await _uow.SaveChangesAsync(user, ct);

        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(entity), "User updated successfully");
    }

    public async Task<ApiResponse<List<UserDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _uow.Users.GetAllAsync(ct);
        return ApiResponse<List<UserDto>>.Ok(users.Select(_mapper.Map<UserDto>).ToList());
    }

    public async Task<ApiResponse<UserDto>> AddAsync(UserDto dto, CancellationToken ct = default)
    {
        return await CreateAsync(dto, null, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Users.GetByIdAsync(id, ct);
        if (entity is null)
            return ApiResponse<bool>.Fail("User not found");

        _uow.Users.Remove(entity); // This does soft delete (sets IsActive = false)
        await _uow.SaveChangesAsync(null, ct);

        return ApiResponse<bool>.Ok(true, "User deleted successfully");
    }

    public async Task<ApiResponse<UserDto>> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var entity = await _uow.Users.GetByUsernameAsync(username, ct);
        if (entity is null)
            return ApiResponse<UserDto>.Fail("User not found");

        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(entity));
    }

    public async Task<ApiResponse<UserDto>> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var entity = await _uow.Users.GetByEmailAsync(email, ct);
        if (entity is null)
            return ApiResponse<UserDto>.Fail("User not found");

        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(entity));
    }
}