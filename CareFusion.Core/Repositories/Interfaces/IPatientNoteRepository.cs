using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing PatientNote entities
/// </summary>
public interface IPatientNoteRepository
{
    /// <summary>
    /// Retrieves a note by its unique ID
    /// </summary>
    /// <param name="id">The note ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The note if found, otherwise null</returns>
    Task<PatientNote?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all notes for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID to get notes for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of notes for the patient</returns>
    Task<IReadOnlyList<PatientNote>> GetByPatientIdAsync(int patientId, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new note to the database
    /// </summary>
    /// <param name="entity">The note entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(PatientNote entity, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing note in the database
    /// </summary>
    /// <param name="entity">The note entity to update</param>
    void Update(PatientNote entity);
    
    /// <summary>
    /// Marks a note as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The note entity to remove</param>
    void Remove(PatientNote entity);
}