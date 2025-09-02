using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing PatientReport entities
/// </summary>
public interface IPatientReportRepository
{
    /// <summary>
    /// Retrieves a report by its unique ID
    /// </summary>
    /// <param name="id">The report ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The report if found, otherwise null</returns>
    Task<PatientReport?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all reports for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID to get reports for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of reports for the patient</returns>
    Task<IReadOnlyList<PatientReport>> GetByPatientIdAsync(int patientId, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new report to the database
    /// </summary>
    /// <param name="entity">The report entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(PatientReport entity, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing report in the database
    /// </summary>
    /// <param name="entity">The report entity to update</param>
    void Update(PatientReport entity);
    
    /// <summary>
    /// Marks a report as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The report entity to remove</param>
    void Remove(PatientReport entity);
}