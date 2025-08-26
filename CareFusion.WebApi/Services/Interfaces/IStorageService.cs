// Placeholder for Services/Interfaces/IStorageService.cs
namespace CareFusion.WebApi.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadExamAsync(Guid examId, Stream file, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadExamAsync(Guid examId, string fileName, CancellationToken ct = default);
}
