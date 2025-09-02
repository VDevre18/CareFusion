using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing PatientReport entities with comprehensive data access operations
/// </summary>
public class PatientReportRepository : IPatientReportRepository
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the PatientReportRepository with the specified database context
    /// </summary>
    /// <param name="db">Database context for patient report data operations</param>
    public PatientReportRepository(CareFusionDbContext db) => _db = db;

    /// <summary>
    /// Retrieves a patient report by its unique integer ID
    /// </summary>
    /// <param name="id">The report ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient report if found, otherwise null</returns>
    public Task<PatientReport?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.PatientReports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves all reports for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID to get reports for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of reports for the patient</returns>
    public async Task<IReadOnlyList<PatientReport>> GetByPatientIdAsync(int patientId, CancellationToken ct = default)
    {
        return await _db.PatientReports.AsNoTracking()
            .Where(r => r.PatientId == patientId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves reports for a specific patient with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to get reports for</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of reports for the patient and total count</returns>
    public async Task<(IReadOnlyList<PatientReport> Items, int Total)> ListByPatientAsync(
        int patientId, int skip, int take, CancellationToken ct = default)
    {
        var q = _db.PatientReports.AsNoTracking()
            .Where(r => r.PatientId == patientId && !r.IsDeleted);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(r => r.CreatedAtUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Searches for patient reports by filename or report type with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to search within</param>
    /// <param name="term">Search term to match against filename or report type</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of reports matching the search criteria and total count</returns>
    public async Task<(IReadOnlyList<PatientReport> Items, int Total)> SearchAsync(
        int patientId, string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _db.PatientReports.AsNoTracking()
            .Where(r => r.PatientId == patientId && !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            q = q.Where(r =>
                r.FileName.ToLower().Contains(lowerTerm) ||
                r.ReportType.ToLower().Contains(lowerTerm) ||
                (r.ContentType != null && r.ContentType.ToLower().Contains(lowerTerm)));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(r => r.CreatedAtUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Retrieves reports by type for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID to get reports for</param>
    /// <param name="reportType">The type of report to filter by</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of reports of the specified type for the patient</returns>
    public async Task<IReadOnlyList<PatientReport>> GetByTypeAsync(
        int patientId, string reportType, CancellationToken ct = default)
    {
        return await _db.PatientReports.AsNoTracking()
            .Where(r => r.PatientId == patientId && r.ReportType == reportType && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Adds a new patient report to the database
    /// </summary>
    /// <param name="entity">The patient report entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public Task AddAsync(PatientReport entity, CancellationToken ct = default)
    {
        _db.PatientReports.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing patient report in the database
    /// </summary>
    /// <param name="entity">The patient report entity to update</param>
    public void Update(PatientReport entity) => _db.PatientReports.Update(entity);

    /// <summary>
    /// Marks a patient report as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The patient report entity to remove</param>
    public void Remove(PatientReport entity)
    {
        entity.IsDeleted = true;
        _db.PatientReports.Update(entity);
    }
}