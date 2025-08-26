// Placeholder for Repositories/PatientRepository.cs
using CareFusion.Core.Entities;
using CareFusion.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareFusion.Core.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly CareFusionDbContext _db;

    public PatientRepository(CareFusionDbContext db) => _db = db;

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Patient?> GetWithExamsAsync(Guid id, CancellationToken ct = default)
        => _db.Patients
              .Include(x => x.Exams.OrderByDescending(e => e.StudyDateUtc))
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByMrnAsync(string mrn, CancellationToken ct = default)
        => _db.Patients.AnyAsync(x => x.MRN == mrn, ct);

    public async Task<(IReadOnlyList<Patient> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default)
    {
        term = term?.Trim();
        var q = _db.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(term))
        {
            q = q.Where(p =>
                (p.FirstName + " " + p.LastName).Contains(term) ||
                (p.LastName + " " + p.FirstName).Contains(term) ||
                (p.MRN ?? "").Contains(term));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                           .Skip(skip).Take(take).ToListAsync(ct);

        return (items, total);
    }

    public Task AddAsync(Patient entity, CancellationToken ct = default)
    {
        _db.Patients.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Patient entity) => _db.Patients.Update(entity);

    public void Remove(Patient entity) => _db.Patients.Remove(entity);

    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken ct = default)
    {
        var patients = await _db.Patients.AsNoTracking().ToListAsync(ct);
        return patients;
    }
}
