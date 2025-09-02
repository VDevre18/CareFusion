using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing Exam entities
/// </summary>
public interface IExamRepository
{
    /// <summary>
    /// Retrieves an exam by its unique ID
    /// </summary>
    /// <param name="id">The exam ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The exam if found, otherwise null</returns>
    Task<Exam?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves an exam with all associated images
    /// </summary>
    /// <param name="id">The exam ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The exam with images if found, otherwise null</returns>
    Task<Exam?> GetWithImagesAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves exams for a specific patient with pagination
    /// </summary>
    /// <param name="patientId">The patient ID to get exams for</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of exams for the patient and total count</returns>
    Task<(IReadOnlyList<Exam> Items, int Total)> ListByPatientAsync(
        int patientId, int skip, int take, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new exam to the database
    /// </summary>
    /// <param name="entity">The exam entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(Exam entity, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing exam in the database
    /// </summary>
    /// <param name="entity">The exam entity to update</param>
    void Update(Exam entity);
    
    /// <summary>
    /// Marks an exam as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The exam entity to remove</param>
    void Remove(Exam entity);
}
