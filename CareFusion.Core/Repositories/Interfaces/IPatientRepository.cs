// Placeholder for Repositories/Interfaces/IPatientRepository.cs
using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Patient?> GetWithExamsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByMrnAsync(string mrn, CancellationToken ct = default);
    Task<(IReadOnlyList<Patient> Items, int Total)> SearchAsync(
        string? term, int skip, int take, CancellationToken ct = default);

    Task AddAsync(Patient entity, CancellationToken ct = default);
    void Update(Patient entity);
    void Remove(Patient entity);
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken ct = default);
}
