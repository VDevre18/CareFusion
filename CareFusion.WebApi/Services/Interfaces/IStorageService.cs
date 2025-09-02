// Placeholder for Services/Interfaces/IStorageService.cs
namespace CareFusion.WebApi.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadExamAsync(int examId, Stream file, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadExamAsync(int examId, string fileName, CancellationToken ct = default);
}
