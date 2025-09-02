using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing Patient entities
/// </summary>
public interface IPatientRepository
{
    /// <summary>
    /// Retrieves a patient by their unique ID
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient if found, otherwise null</returns>
    Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves a patient with all their associated exams
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient with exams if found, otherwise null</returns>
    Task<Patient?> GetWithExamsAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves a patient with all their associated notes and reports
    /// </summary>
    /// <param name="id">The patient ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The patient with notes and reports if found, otherwise null</returns>
    Task<Patient?> GetWithNotesAndReportsAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Checks if a patient exists with the given Medical Record Number
    /// </summary>
    /// <param name="mrn">The Medical Record Number to check</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if a patient exists with the MRN, otherwise false</returns>
    Task<bool> ExistsByMrnAsync(string mrn, CancellationToken ct = default);
    
    /// <summary>
    /// Searches for patients based on name or patient ID with pagination and clinic site filtering
    /// </summary>
    /// <param name="term">Search term (name or ID)</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <param name="clinicSiteId">Optional clinic site ID to filter patients</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of patients matching the search criteria and total count</returns>
    Task<(IReadOnlyList<Patient> Items, int Total)> SearchAsync(
        string? term, int skip, int take, int? clinicSiteId = null, CancellationToken ct = default);

    /// <summary>
    /// Adds a new patient to the database
    /// </summary>
    /// <param name="entity">The patient entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(Patient entity, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing patient in the database
    /// </summary>
    /// <param name="entity">The patient entity to update</param>
    void Update(Patient entity);
    
    /// <summary>
    /// Marks a patient as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The patient entity to remove</param>
    void Remove(Patient entity);
    
    /// <summary>
    /// Retrieves all active patients
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all active patients</returns>
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all deleted patients
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all deleted patients</returns>
    Task<IReadOnlyList<Patient>> GetDeletedAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Finds potential duplicate patients based on name, DOB, email, or phone
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Groups of potentially duplicate patients</returns>
    Task<IReadOnlyList<IGrouping<string, Patient>>> GetPotentialDuplicatesAsync(CancellationToken ct = default);
}
