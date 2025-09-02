using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing PatientNote entities with comprehensive data access operations
/// </summary>
public class PatientNoteRepository : IPatientNoteRepository
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the PatientNoteRepository with the specified database context
    /// </summary>
    /// <param name="db">Database context for patient note data operations</param>
    public PatientNoteRepository(CareFusionDbContext db) => _db = db;

    /// <summary>
    /// Retrieves a patient note by its unique integer ID
    /// </summary>
    /// <param name="id">The note ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient note if found, otherwise null</returns>
    public Task<PatientNote?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.PatientNotes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves all notes for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID to get notes for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of notes for the patient</returns>
    public async Task<IReadOnlyList<PatientNote>> GetByPatientIdAsync(int patientId, CancellationToken ct = default)
    {
        return await _db.PatientNotes.AsNoTracking()
            .Where(n => n.PatientId == patientId && !n.IsDeleted)
            .OrderByDescending(n => n.NoteDate)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves notes for a specific patient with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to get notes for</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of notes for the patient and total count</returns>
    public async Task<(IReadOnlyList<PatientNote> Items, int Total)> ListByPatientAsync(
        int patientId, int skip, int take, CancellationToken ct = default)
    {
        var q = _db.PatientNotes.AsNoTracking()
            .Where(n => n.PatientId == patientId && !n.IsDeleted);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(n => n.NoteDate)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Searches for patient notes by content or author with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to search within</param>
    /// <param name="term">Search term to match against note content or author</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of notes matching the search criteria and total count</returns>
    public async Task<(IReadOnlyList<PatientNote> Items, int Total)> SearchAsync(
        int patientId, string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _db.PatientNotes.AsNoTracking()
            .Where(n => n.PatientId == patientId && !n.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            q = q.Where(n =>
                n.Content.ToLower().Contains(lowerTerm) ||
                (n.AuthorName != null && n.AuthorName.ToLower().Contains(lowerTerm)) ||
                n.NoteType.ToLower().Contains(lowerTerm));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(n => n.NoteDate)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Adds a new patient note to the database
    /// </summary>
    /// <param name="entity">The patient note entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public Task AddAsync(PatientNote entity, CancellationToken ct = default)
    {
        _db.PatientNotes.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing patient note in the database
    /// </summary>
    /// <param name="entity">The patient note entity to update</param>
    public void Update(PatientNote entity) => _db.PatientNotes.Update(entity);

    /// <summary>
    /// Marks a patient note as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The patient note entity to remove</param>
    public void Remove(PatientNote entity)
    {
        entity.IsDeleted = true;
        _db.PatientNotes.Update(entity);
    }
}