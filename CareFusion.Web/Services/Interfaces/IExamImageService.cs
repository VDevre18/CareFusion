using CareFusion.Model.Dtos;

namespace CareFusion.Web.Services.Interfaces;

/// <summary>
/// Service interface for managing exam images in the web layer
/// </summary>
public interface IExamImageService
{
    /// <summary>
    /// Retrieves all exam images with patient and exam details for the image cloud view
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter images</param>
    /// <param name="modality">Optional modality filter</param>
    /// <param name="patientId">Optional patient ID filter</param>
    /// <param name="clinicSiteId">Optional clinic site filter</param>
    /// <param name="skip">Number of records to skip for pagination</param>
    /// <param name="take">Number of records to take for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of exam image DTOs with associated data</returns>
    Task<(IReadOnlyList<ExamImageDto> Items, int Total)> GetImagesAsync(
        string? searchTerm = null,
        string? modality = null,
        int? patientId = null,
        int? clinicSiteId = null,
        int skip = 0,
        int take = 50,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves a specific exam image by ID
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Exam image DTO if found, null otherwise</returns>
    Task<ExamImageDto?> GetImageAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves images for a specific exam
    /// </summary>
    /// <param name="examId">Exam ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of exam image DTOs</returns>
    Task<IReadOnlyList<ExamImageDto>> GetImagesByExamAsync(int examId, CancellationToken ct = default);

    /// <summary>
    /// Soft deletes an exam image
    /// </summary>
    /// <param name="id">Image ID to delete</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteImageAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Uploads a new image for an exam
    /// </summary>
    /// <param name="examId">Exam ID</param>
    /// <param name="fileName">Original filename</param>
    /// <param name="contentType">MIME type</param>
    /// <param name="fileData">Binary file data</param>
    /// <param name="uploadedBy">User who uploaded the image</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created exam image DTO</returns>
    Task<ExamImageDto?> UploadImageAsync(
        int examId,
        string fileName,
        string contentType,
        byte[] fileData,
        string? uploadedBy = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the binary data for an image file
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Binary file data and content type</returns>
    Task<(byte[] Data, string ContentType)?> GetImageDataAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Gets the thumbnail data for an image file
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Binary thumbnail data and content type</returns>
    Task<(byte[] Data, string ContentType)?> GetThumbnailDataAsync(int id, CancellationToken ct = default);
}