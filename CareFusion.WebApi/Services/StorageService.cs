// Placeholder for Services/StorageService.cs
using CareFusion.WebApi.Infrastructure;
using CareFusion.WebApi.Services.Interfaces;

namespace CareFusion.WebApi.Services;

public class StorageService : IStorageService
{
    private readonly AzureStorageHelper _storageHelper;
    private readonly IConfiguration _config;

    public StorageService(IConfiguration config)
    {
        _config = config;
        var connectionString = _config.GetConnectionString("AzureBlobStorage");
        _storageHelper = new AzureStorageHelper(connectionString!);
    }

    public async Task<string> UploadExamAsync(Guid examId, Stream file, string fileName, string contentType, CancellationToken ct = default)
    {
        var containerName = "exams";
        var blobName = $"{examId}/{fileName}";

        return await _storageHelper.UploadAsync(containerName, blobName, file, contentType, ct);
    }

    public async Task<Stream> DownloadExamAsync(Guid examId, string fileName, CancellationToken ct = default)
    {
        var containerName = "exams";
        var blobName = $"{examId}/{fileName}";

        return await _storageHelper.DownloadAsync(containerName, blobName, ct);
    }
}
