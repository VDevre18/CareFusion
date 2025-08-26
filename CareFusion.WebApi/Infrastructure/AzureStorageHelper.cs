// Placeholder for Infrastructure/AzureStorageHelper.cs
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CareFusion.WebApi.Infrastructure;

public class AzureStorageHelper
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageHelper(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadAsync(string containerName, string fileName, Stream stream, string contentType, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(fileName);
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blob.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string containerName, string fileName, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(fileName);

        if (await blob.ExistsAsync(ct))
        {
            var response = await blob.DownloadStreamingAsync(cancellationToken: ct);
            return response.Value.Content;
        }

        throw new FileNotFoundException($"Blob {fileName} not found in {containerName}");
    }
}

