using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing Exam entities with comprehensive data access operations
/// </summary>
public class ExamRepository : IExamRepository
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the ExamRepository with the specified database context
    /// </summary>
    /// <param name="db">Database context for exam data operations</param>
    public ExamRepository(CareFusionDbContext db) => _db = db;

    /// <summary>
    /// Retrieves an exam by its unique integer ID
    /// </summary>
    /// <param name="id">The exam ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The exam if found, otherwise null</returns>
    public Task<Exam?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Exams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves an exam with all associated images
    /// </summary>
    /// <param name="id">The exam ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The exam with images if found, otherwise null</returns>
    public Task<Exam?> GetWithImagesAsync(int id, CancellationToken ct = default)
        => _db.Exams
              .Include(x => x.Images.OrderBy(i => i.CreatedAtUtc))
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves exams for a specific patient with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to get exams for</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of exams for the patient and total count</returns>
    public async Task<(IReadOnlyList<Exam> Items, int Total)> ListByPatientAsync(
        int patientId, int skip, int take, CancellationToken ct = default)
    {
        var q = _db.Exams.AsNoTracking()
            .Where(e => e.PatientId == patientId && !e.IsDeleted);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(e => e.StudyDateUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Adds a new exam to the database
    /// </summary>
    /// <param name="entity">The exam entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public Task AddAsync(Exam entity, CancellationToken ct = default)
    {
        _db.Exams.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing exam in the database
    /// </summary>
    /// <param name="entity">The exam entity to update</param>
    public void Update(Exam entity) => _db.Exams.Update(entity);

    /// <summary>
    /// Marks an exam as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The exam entity to remove</param>
    public void Remove(Exam entity)
    {
        entity.IsDeleted = true;
        _db.Exams.Update(entity);
    }
}
