// Placeholder for Repositories/Interfaces/IUnitOfWork.cs
namespace CareFusion.Core.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IPatientRepository Patients { get; }
    IExamRepository Exams { get; }

    Task<int> SaveChangesAsync(string? user = null, CancellationToken ct = default);
}
