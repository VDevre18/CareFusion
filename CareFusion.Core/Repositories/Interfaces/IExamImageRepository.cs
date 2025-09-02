using CareFusion.Core.Entities;

namespace CareFusion.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing ExamImage entities
/// </summary>
public interface IExamImageRepository
{
    /// <summary>
    /// Retrieves an image by its unique ID
    /// </summary>
    /// <param name="id">The image ID to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The image if found, otherwise null</returns>
    Task<ExamImage?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves all images for a specific exam
    /// </summary>
    /// <param name="examId">The exam ID to get images for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images for the exam</returns>
    Task<IReadOnlyList<ExamImage>> GetByExamIdAsync(int examId, CancellationToken ct = default);
    
    /// <summary>
    /// Adds a new image to the database
    /// </summary>
    /// <param name="entity">The image entity to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(ExamImage entity, CancellationToken ct = default);
    
    /// <summary>
    /// Updates an existing image in the database
    /// </summary>
    /// <param name="entity">The image entity to update</param>
    void Update(ExamImage entity);
    
    /// <summary>
    /// Marks an image as deleted (soft delete)
    /// </summary>
    /// <param name="entity">The image entity to remove</param>
    void Remove(ExamImage entity);
    
    /// <summary>
    /// Retrieves all deleted images
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all deleted images</returns>
    Task<IReadOnlyList<ExamImage>> GetDeletedAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves all images for a specific exam (alias for GetByExamIdAsync)
    /// </summary>
    /// <param name="examId">The exam ID to get images for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of images for the exam</returns>
    Task<IReadOnlyList<ExamImage>> GetAllByExamAsync(int examId, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all non-deleted exam images with exam and patient data loaded
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all images with related data</returns>
    Task<IReadOnlyList<ExamImage>> GetAllWithExamAndPatientAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves an exam image by ID with exam and patient data loaded
    /// </summary>
    /// <param name="id">The image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The image with related data if found, otherwise null</returns>
    Task<ExamImage?> GetByIdWithExamAndPatientAsync(int id, CancellationToken ct = default);
}