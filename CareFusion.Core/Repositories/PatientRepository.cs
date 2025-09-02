using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing Patient entities with comprehensive data access operations
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the PatientRepository with the specified database context
    /// </summary>
    /// <param name="db">Database context for patient data operations</param>
    public PatientRepository(CareFusionDbContext db) => _db = db;

    /// <summary>
    /// Retrieves a patient by their unique integer ID
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient if found, otherwise null</returns>
    public Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves a patient with all associated exams ordered by study date
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient with exams if found, otherwise null</returns>
    public Task<Patient?> GetWithExamsAsync(int id, CancellationToken ct = default)
        => _db.Patients
              .Include(x => x.Exams.OrderByDescending(e => e.StudyDateUtc))
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves a patient with all associated notes and reports
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient with notes and reports if found, otherwise null</returns>
    public Task<Patient?> GetWithNotesAndReportsAsync(int id, CancellationToken ct = default)
        => _db.Patients
              .Include(x => x.Notes)
              .Include(x => x.Reports)
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Checks if a patient exists with the specified MRN (Medical Record Number)
    /// </summary>
    /// <param name="mrn">Medical Record Number to check</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if a patient exists with the MRN, false otherwise</returns>
    public Task<bool> ExistsByMrnAsync(string mrn, CancellationToken ct = default)
        => _db.Patients.AnyAsync(x => x.MRN == mrn, ct);

    /// <summary>
    /// Searches for patients by name, MRN, or integer ID with pagination and clinic site filtering
    /// </summary>
    /// <param name="term">Search term to match against patient data</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="clinicSiteId">Optional clinic site ID to filter patients</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of matching patients and total count</returns>
    public async Task<(IReadOnlyList<Patient> Items, int Total)> SearchAsync(
        string? term, int skip, int take, int? clinicSiteId = null, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _db.Patients.AsNoTracking().Where(p => !p.IsDeleted);

        // Filter by clinic site if specified
        if (clinicSiteId.HasValue)
        {
            q = q.Where(p => p.ClinicSiteId == clinicSiteId.Value);
        }

        if (!string.IsNullOrWhiteSpace(term))
        {
            // Support searching by integer ID as well as name and MRN
            var isNumeric = int.TryParse(term, out var searchId);
            
            q = q.Where(p =>
                (isNumeric && p.Id == searchId) ||
                (p.FirstName + " " + p.LastName).Contains(term) ||
                (p.LastName + " " + p.FirstName).Contains(term) ||
                (p.MRN ?? "").Contains(term) ||
                (p.Phone ?? "").Contains(term) ||
                (p.Email ?? "").Contains(term));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(p => p.Id)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Retrieves all active patients from the database
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active patients</returns>
    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken ct = default)
    {
        var patients = await _db.Patients
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id)
            .ToListAsync(ct);
        return patients;
    }

    /// <summary>
    /// Adds a new patient to the database
    /// </summary>
    /// <param name="entity">The patient entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public Task AddAsync(Patient entity, CancellationToken ct = default)
    {
        _db.Patients.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing patient in the database
    /// </summary>
    /// <param name="entity">The patient entity to update</param>
    public void Update(Patient entity) => _db.Patients.Update(entity);

    /// <summary>
    /// Marks a patient as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The patient entity to remove</param>
    public void Remove(Patient entity)
    {
        entity.IsDeleted = true;
        _db.Patients.Update(entity);
    }

    /// <summary>
    /// Retrieves all deleted patients from the database
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all deleted patients</returns>
    public async Task<IReadOnlyList<Patient>> GetDeletedAsync(CancellationToken ct = default)
    {
        var patients = await _db.Patients
            .AsNoTracking()
            .Where(p => p.IsDeleted)
            .OrderByDescending(p => p.ModifiedAtUtc)
            .ToListAsync(ct);
        return patients;
    }

    /// <summary>
    /// Finds potential duplicate patients based on name, DOB, email, or phone
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Groups of potentially duplicate patients</returns>
    public async Task<IReadOnlyList<IGrouping<string, Patient>>> GetPotentialDuplicatesAsync(CancellationToken ct = default)
    {
        var patients = await _db.Patients
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ToListAsync(ct);

        // Group by combination of first name, last name, and DOB
        var nameAndDobDuplicates = patients
            .Where(p => !string.IsNullOrEmpty(p.FirstName) && !string.IsNullOrEmpty(p.LastName) && p.DateOfBirth.HasValue)
            .GroupBy(p => $"{p.FirstName?.ToLower()}-{p.LastName?.ToLower()}-{p.DateOfBirth?.ToString("yyyy-MM-dd")}")
            .Where(g => g.Count() > 1)
            .ToList();

        // Group by email (if not null/empty)
        var emailDuplicates = patients
            .Where(p => !string.IsNullOrEmpty(p.Email))
            .GroupBy(p => p.Email!.ToLower())
            .Where(g => g.Count() > 1)
            .ToList();

        // Group by phone (if not null/empty)
        var phoneDuplicates = patients
            .Where(p => !string.IsNullOrEmpty(p.Phone))
            .GroupBy(p => p.Phone!)
            .Where(g => g.Count() > 1)
            .ToList();

        // Combine all duplicates
        var allDuplicates = new List<IGrouping<string, Patient>>();
        allDuplicates.AddRange(nameAndDobDuplicates);
        allDuplicates.AddRange(emailDuplicates);
        allDuplicates.AddRange(phoneDuplicates);

        return allDuplicates;
    }
}
