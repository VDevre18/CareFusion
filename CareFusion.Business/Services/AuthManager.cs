// Placeholder for Services/AuthManager.cs
using CareFusion.Core.Repositories.Interfaces;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;

namespace CareFusion.Business.Services;

public class AuthManager
{
    private readonly IUnitOfWork _uow;

    public AuthManager(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public virtual async Task<ApiResponse<UserDto>> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(username))
            return ApiResponse<UserDto>.Fail("Username is required");

        if (string.IsNullOrEmpty(password))
            return ApiResponse<UserDto>.Fail("Password is required");

        // Remove hardcoded admin check - all users should be from database now

        // Try to find user in database
        var user = await _uow.Users.GetByUsernameAsync(username, ct);
        if (user == null)
            return ApiResponse<UserDto>.Fail("Invalid username or password");

        if (!user.IsActive)
            return ApiResponse<UserDto>.Fail("User account is inactive");

        // Verify the password using BCrypt
        try
        {
            // First, check if the password is already hashed with BCrypt
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Password verification successful with BCrypt
            }
            else if (user.PasswordHash == password)
            {
                // Fallback for legacy plain text passwords (should be migrated)
                Console.WriteLine($"Warning: User '{username}' has unhashed password. Consider migrating to BCrypt.");
            }
            else
            {
                return ApiResponse<UserDto>.Fail("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            // If BCrypt verification fails (e.g., malformed hash), try plain text comparison as fallback
            Console.WriteLine($"BCrypt verification failed for user '{username}': {ex.Message}");
            if (user.PasswordHash != password)
                return ApiResponse<UserDto>.Fail("Invalid username or password");
        }

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role ?? "User",
            IsActive = user.IsActive,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DefaultClinicSiteId = user.DefaultClinicSiteId
        };

        return ApiResponse<UserDto>.Ok(dto);
    }
}
