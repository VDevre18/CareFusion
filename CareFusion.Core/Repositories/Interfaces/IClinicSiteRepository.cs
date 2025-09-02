using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing ClinicSite entities
/// </summary>
public interface IClinicSiteRepository
{
    /// <summary>
    /// Retrieves a clinic site by its unique ID
    /// </summary>
    /// <param name="id">The clinic site ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The clinic site if found, otherwise null</returns>
    Task<ClinicSite?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves a clinic site by its code
    /// </summary>
    /// <param name="code">The clinic site code to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The clinic site if found, otherwise null</returns>
    Task<ClinicSite?> GetByCodeAsync(string code, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all active clinic sites
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active clinic sites</returns>
    Task<IReadOnlyList<ClinicSite>> GetAllAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Searches for clinic sites by name or code
    /// </summary>
    /// <param name="term">Search term</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of clinic sites matching the search criteria and total count</returns>
    Task<(IReadOnlyList<ClinicSite> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new clinic site to the database
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(ClinicSite clinicSite, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing clinic site in the database
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to update</param>
    void Update(ClinicSite clinicSite);
    
    /// <summary>
    /// Marks a clinic site as deleted (soft delete)
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to remove</param>
    void Remove(ClinicSite clinicSite);
}