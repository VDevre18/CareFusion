using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

/// <summary>
/// Repository implementation for managing ExamImage entities with comprehensive data access operations
/// </summary>
public class ExamImageRepository : IExamImageRepository
{
    private readonly CareFusionDbContext _db;

    /// <summary>
    /// Initializes a new instance of the ExamImageRepository with the specified database context
    /// </summary>
    /// <param name="db">Database context for exam image data operations</param>
    public ExamImageRepository(CareFusionDbContext db) => _db = db;

    /// <summary>
    /// Retrieves an exam image by its unique integer ID
    /// </summary>
    /// <param name="id">The image ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The exam image if found, otherwise null</returns>
    public Task<ExamImage?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.ExamImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Retrieves images for a specific exam with pagination
    /// </summary>
    /// <param name="examId">The exam ID to get images for</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images for the exam and total count</returns>
    public async Task<(IReadOnlyList<ExamImage> Items, int Total)> ListByExamAsync(
        int examId, int skip, int take, CancellationToken ct = default)
    {
        var q = _db.ExamImages.AsNoTracking()
            .Where(i => i.ExamId == examId && !i.IsDeleted);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(i => i.CreatedAtUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Retrieves all images for a specific exam
    /// </summary>
    /// <param name="examId">The exam ID to get images for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images for the exam</returns>
    public async Task<IReadOnlyList<ExamImage>> GetByExamIdAsync(int examId, CancellationToken ct = default)
    {
        return await _db.ExamImages.AsNoTracking()
            .Where(i => i.ExamId == examId && !i.IsDeleted)
            .OrderBy(i => i.CreatedAtUtc)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves all images for a specific exam
    /// </summary>
    /// <param name="examId">The exam ID to get images for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all images for the exam</returns>
    public async Task<IReadOnlyList<ExamImage>> GetAllByExamAsync(int examId, CancellationToken ct = default)
    {
        return await _db.ExamImages.AsNoTracking()
            .Where(i => i.ExamId == examId && !i.IsDeleted)
            .OrderBy(i => i.CreatedAtUtc)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets the count of images for a specific exam
    /// </summary>
    /// <param name="examId">The exam ID to count images for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of images for the exam</returns>
    public async Task<int> GetCountByExamAsync(int examId, CancellationToken ct = default)
    {
        return await _db.ExamImages
            .Where(i => i.ExamId == examId && !i.IsDeleted)
            .CountAsync(ct);
    }

    /// <summary>
    /// Searches for exam images by filename with pagination
    /// </summary>
    /// <param name="examId">The exam ID to search within</param>
    /// <param name="term">Search term to match against filename</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images matching the search criteria and total count</returns>
    public async Task<(IReadOnlyList<ExamImage> Items, int Total)> SearchAsync(
        int examId, string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _db.ExamImages.AsNoTracking()
            .Where(i => i.ExamId == examId && !i.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            q = q.Where(i => i.FileName.ToLower().Contains(lowerTerm));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(i => i.CreatedAtUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    /// <summary>
    /// Adds a new exam image to the database
    /// </summary>
    /// <param name="entity">The exam image entity to add</param>
    /// <param name="ct">Cancellation token</param>
    public Task AddAsync(ExamImage entity, CancellationToken ct = default)
    {
        _db.ExamImages.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing exam image in the database
    /// </summary>
    /// <param name="entity">The exam image entity to update</param>
    public void Update(ExamImage entity) => _db.ExamImages.Update(entity);

    /// <summary>
    /// Marks an exam image as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The exam image entity to remove</param>
    public void Remove(ExamImage entity)
    {
        entity.IsDeleted = true;
        _db.ExamImages.Update(entity);
    }

    /// <summary>
    /// Retrieves all deleted exam images from the database
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all deleted exam images</returns>
    public async Task<IReadOnlyList<ExamImage>> GetDeletedAsync(CancellationToken ct = default)
    {
        var images = await _db.ExamImages
            .Include(i => i.Exam)
            .ThenInclude(e => e.Patient)
            .AsNoTracking()
            .Where(i => i.IsDeleted)
            .OrderByDescending(i => i.ModifiedAtUtc)
            .ToListAsync(ct);
        return images;
    }

    /// <summary>
    /// Retrieves all non-deleted exam images with exam and patient data loaded
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all images with related data</returns>
    public async Task<IReadOnlyList<ExamImage>> GetAllWithExamAndPatientAsync(CancellationToken ct = default)
    {
        return await _db.ExamImages
            .Include(i => i.Exam)
            .ThenInclude(e => e.Patient)
            .AsNoTracking()
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves an exam image by ID with exam and patient data loaded
    /// </summary>
    /// <param name="id">The image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The image with related data if found, otherwise null</returns>
    public async Task<ExamImage?> GetByIdWithExamAndPatientAsync(int id, CancellationToken ct = default)
    {
        return await _db.ExamImages
            .Include(i => i.Exam)
            .ThenInclude(e => e.Patient)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, ct);
    }
}