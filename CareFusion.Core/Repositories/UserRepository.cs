using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing User entities with comprehensive data access operations
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly CareFusionDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository with the specified database context
    /// </summary>
    /// <param name="context">Database context for user data operations</param>
    public UserRepository(CareFusionDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a user by their username
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive, ct);
    }

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, ct);
    }

    /// <summary>
    /// Retrieves a user by their unique integer ID
    /// </summary>
    /// <param name="id">The user ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    /// <summary>
    /// Retrieves all active users
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active users</returns>
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Searches for users by name or username with pagination
    /// </summary>
    /// <param name="term">Search term to match against user data</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of users matching the search criteria and total count</returns>
    public async Task<(IReadOnlyList<User> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _context.Users.AsNoTracking().Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            q = q.Where(u =>
                u.Username.ToLower().Contains(lowerTerm) ||
                u.Email.ToLower().Contains(lowerTerm) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(lowerTerm)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(lowerTerm)));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(u => u.Username)
                           .Skip(skip).Take(take)
                           .ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Adds a new user to the database
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
    }

    /// <summary>
    /// Updates an existing user in the database
    /// </summary>
    /// <param name="user">The user entity to update</param>
    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    /// <summary>
    /// Marks a user as deleted (soft delete)
    /// </summary>
    /// <param name="user">The user entity to remove</param>
    public void Remove(User user)
    {
        user.IsActive = false;
        _context.Users.Update(user);
    }
}