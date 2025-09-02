using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing ClinicSite entities with comprehensive data access operations
/// </summary>
public class ClinicSiteRepository : IClinicSiteRepository
{
    private readonly CareFusionDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ClinicSiteRepository with the specified database context
    /// </summary>
    /// <param name="context">Database context for clinic site data operations</param>
    public ClinicSiteRepository(CareFusionDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a clinic site by its unique integer ID
    /// </summary>
    /// <param name="id">The clinic site ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The clinic site if found, otherwise null</returns>
    public async Task<ClinicSite?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.ClinicSites
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    /// <summary>
    /// Retrieves a clinic site by its code
    /// </summary>
    /// <param name="code">The clinic site code to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The clinic site if found, otherwise null</returns>
    public async Task<ClinicSite?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.ClinicSites
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive, ct);
    }

    /// <summary>
    /// Retrieves all active clinic sites
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active clinic sites</returns>
    public async Task<IReadOnlyList<ClinicSite>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.ClinicSites
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Searches for clinic sites by name, code, or city with pagination
    /// </summary>
    /// <param name="term">Search term to match against clinic site data</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of clinic sites matching the search criteria and total count</returns>
    public async Task<(IReadOnlyList<ClinicSite> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _context.ClinicSites.AsNoTracking().Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            q = q.Where(c =>
                c.Name.ToLower().Contains(lowerTerm) ||
                c.Code.ToLower().Contains(lowerTerm) ||
                (c.City != null && c.City.ToLower().Contains(lowerTerm)) ||
                (c.Address != null && c.Address.ToLower().Contains(lowerTerm)));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(c => c.Name)
                           .Skip(skip).Take(take)
                           .ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Adds a new clinic site to the database
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public async Task AddAsync(ClinicSite clinicSite, CancellationToken ct = default)
    {
        await _context.ClinicSites.AddAsync(clinicSite, ct);
    }

    /// <summary>
    /// Updates an existing clinic site in the database
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to update</param>
    public void Update(ClinicSite clinicSite)
    {
        _context.ClinicSites.Update(clinicSite);
    }

    /// <summary>
    /// Marks a clinic site as deleted (soft delete)
    /// </summary>
    /// <param name="clinicSite">The clinic site entity to remove</param>
    public void Remove(ClinicSite clinicSite)
    {
        clinicSite.IsActive = false;
        _context.ClinicSites.Update(clinicSite);
    }
}