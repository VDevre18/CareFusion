// Placeholder for Repositories/ExamRepository.cs
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly CareFusionDbContext _db;

    public ExamRepository(CareFusionDbContext db) => _db = db;

    public Task<Exam?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Exams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<Exam> Items, int Total)> ListByPatientAsync(
        Guid patientId, int skip, int take, CancellationToken ct = default)
    {
        var q = _db.Exams.AsNoTracking().Where(e => e.PatientId == patientId);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(e => e.StudyDateUtc)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    public Task AddAsync(Exam entity, CancellationToken ct = default)
    {
        _db.Exams.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Exam entity) => _db.Exams.Update(entity);

    public void Remove(Exam entity) => _db.Exams.Remove(entity);
}
