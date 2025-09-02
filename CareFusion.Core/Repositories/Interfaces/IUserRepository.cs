using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing User entities
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their username
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves a user by their unique ID
    /// </summary>
    /// <param name="id">The user ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all active users
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active users</returns>
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Searches for users by name or username
    /// </summary>
    /// <param name="term">Search term</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of users matching the search criteria and total count</returns>
    Task<(IReadOnlyList<User> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new user to the database
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(User user, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing user in the database
    /// </summary>
    /// <param name="user">The user entity to update</param>
    void Update(User user);
    
    /// <summary>
    /// Marks a user as deleted (soft delete)
    /// </summary>
    /// <param name="user">The user entity to remove</param>
    void Remove(User user);
}