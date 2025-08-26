// Placeholder for Repositories/UnitOfWork.cs
using CareFusion.Core.Repositories.Interfaces;

namespace CareFusion.Core.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CareFusionDbContext _db;

    public UnitOfWork(CareFusionDbContext db,
                      IPatientRepository patients,
                      IExamRepository exams)
    {
        _db = db;
        Patients = patients;
        Exams = exams;
    }

    public IPatientRepository Patients { get; }
    public IExamRepository Exams { get; }

    public Task<int> SaveChangesAsync(string? user = null, CancellationToken ct = default)
        => _db.SaveChangesAsync(user, ct);

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}
