using System.Net.Http.Json;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.Web.Services.Interfaces;

namespace CareFusion.Web.Services;

/// <summary>
/// Service implementation for managing exam images in the web layer
/// </summary>
public class ExamImageService : IExamImageService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<ExamImageService> _logger;

    public ExamImageService(IHttpClientFactory httpFactory, ILogger<ExamImageService> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all exam images with patient and exam details for the image cloud view
    /// </summary>
    public async Task<(IReadOnlyList<ExamImageDto> Items, int Total)> GetImagesAsync(
        string? searchTerm = null,
        string? modality = null,
        int? patientId = null,
        int? clinicSiteId = null,
        int skip = 0,
        int take = 50,
        CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var queryParams = $"searchTerm={Uri.EscapeDataString(searchTerm ?? "")}&skip={skip}&take={take}";
            
            if (!string.IsNullOrWhiteSpace(modality))
                queryParams += $"&modality={Uri.EscapeDataString(modality)}";
            if (patientId.HasValue)
                queryParams += $"&patientId={patientId.Value}";
            if (clinicSiteId.HasValue)
                queryParams += $"&clinicSiteId={clinicSiteId.Value}";

            var response = await http.GetFromJsonAsync<ApiResponse<PagedResult<ExamImageDto>>>(
                $"api/examimages?{queryParams}", ct);

            var result = response?.Data ?? new PagedResult<ExamImageDto>();
            return (result.Items, result.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving images from API");
            return (new List<ExamImageDto>(), 0);
        }
    }

    /// <summary>
    /// Retrieves a specific exam image by ID
    /// </summary>
    public async Task<ExamImageDto?> GetImageAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<ExamImageDto>>(
                $"api/examimages/{id}", ct);
            return response?.Data;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image {ImageId} from API", id);
            return null;
        }
    }

    /// <summary>
    /// Retrieves images for a specific exam
    /// </summary>
    public async Task<IReadOnlyList<ExamImageDto>> GetImagesByExamAsync(int examId, CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetFromJsonAsync<ApiResponse<IReadOnlyList<ExamImageDto>>>(
                $"api/examimages/exam/{examId}", ct);
            return response?.Data ?? new List<ExamImageDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving images for exam {ExamId} from API", examId);
            return new List<ExamImageDto>();
        }
    }

    /// <summary>
    /// Soft deletes an exam image
    /// </summary>
    public async Task<bool> DeleteImageAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.DeleteAsync($"api/examimages/{id}", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImageId} via API", id);
            return false;
        }
    }

    /// <summary>
    /// Uploads a new image for an exam
    /// </summary>
    public async Task<ExamImageDto?> UploadImageAsync(
        int examId,
        string fileName,
        string contentType,
        byte[] fileData,
        string? uploadedBy = null,
        CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(examId.ToString()), "ExamId");
            formData.Add(new StringContent(fileName), "FileName");
            formData.Add(new StringContent(contentType), "ContentType");
            if (!string.IsNullOrEmpty(uploadedBy))
                formData.Add(new StringContent(uploadedBy), "UploadedBy");
            
            formData.Add(new ByteArrayContent(fileData), "File", fileName);

            var response = await http.PostAsync("api/examimages/upload", formData, ct);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ExamImageDto>>(cancellationToken: ct);
                return apiResponse?.Data;
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for exam {ExamId} via API", examId);
            return null;
        }
    }

    /// <summary>
    /// Gets the binary data for an image file
    /// </summary>
    public async Task<(byte[] Data, string ContentType)?> GetImageDataAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetAsync($"api/examimages/{id}/data", ct);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync(ct);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return (data, contentType);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image data for {ImageId} via API", id);
            return null;
        }
    }

    /// <summary>
    /// Gets the thumbnail data for an image file
    /// </summary>
    public async Task<(byte[] Data, string ContentType)?> GetThumbnailDataAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var response = await http.GetAsync($"api/examimages/{id}/thumbnail", ct);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync(ct);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return (data, contentType);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving thumbnail for {ImageId} via API", id);
            return null;
        }
    }
}