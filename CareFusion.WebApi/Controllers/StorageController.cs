// Placeholder for Controllers/StorageController.cs
using CareFusion.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareFusion.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;

    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost("upload/{examId:guid}")]
    public async Task<IActionResult> Upload(Guid examId, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        await using var stream = file.OpenReadStream();
        var url = await _storageService.UploadExamAsync(examId, stream, file.FileName, file.ContentType, ct);

        return Ok(new { Url = url });
    }

    [HttpGet("download/{examId:guid}/{fileName}")]
    public async Task<IActionResult> Download(Guid examId, string fileName, CancellationToken ct)
    {
        var stream = await _storageService.DownloadExamAsync(examId, fileName, ct);
        return File(stream, "application/octet-stream", fileName);
    }
}
