// Placeholder for Repositories/Interfaces/IExamRepository.cs
using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

public interface IExamRepository
{
    Task<Exam?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Exam> Items, int Total)> ListByPatientAsync(
        Guid patientId, int skip, int take, CancellationToken ct = default);
    Task AddAsync(Exam entity, CancellationToken ct = default);
    void Update(Exam entity);
    void Remove(Exam entity);
}
