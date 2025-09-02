using CareFusion.WebApi.Controllers;
using CareFusion.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;
using Xunit;

namespace CareFusion.WebApi.Tests;

public class StorageControllerTests
{
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly StorageController _controller;
    private readonly CancellationToken _cancellationToken;

    public StorageControllerTests()
    {
        _mockStorageService = new Mock<IStorageService>();
        _controller = new StorageController(_mockStorageService.Object);
        _cancellationToken = CancellationToken.None;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Upload_WithValidFile_ShouldReturnOkWithUrl()
    {
        // Arrange
        var examId = 1;
        var fileName = "test-image.jpg";
        var contentType = "image/jpeg";
        var fileContent = "fake file content";
        var fileBytes = Encoding.UTF8.GetBytes(fileContent);
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(fileBytes.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileBytes));

        var expectedUrl = $"https://storage.example.com/exams/{examId}/{fileName}";
        
        _mockStorageService
            .Setup(x => x.UploadExamAsync(examId, It.IsAny<Stream>(), fileName, contentType, _cancellationToken))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _controller.Upload(examId, mockFile.Object, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        
        // Use reflection to access anonymous type properties
        var urlProperty = response!.GetType().GetProperty("Url");
        var actualUrl = urlProperty!.GetValue(response)?.ToString();
        Assert.Equal(expectedUrl, actualUrl);
        
        // Verify the service was called with correct parameters
        _mockStorageService.Verify(x => x.UploadExamAsync(
            examId, 
            It.IsAny<Stream>(), 
            fileName, 
            contentType, 
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Upload_WithNullFile_ShouldReturnBadRequest()
    {
        // Arrange
        var examId = 2;
        IFormFile? file = null;

        // Act
        var result = await _controller.Upload(examId, file!, _cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is required", badRequestResult.Value);
        
        // Verify the service was not called
        _mockStorageService.Verify(x => x.UploadExamAsync(
            It.IsAny<int>(), 
            It.IsAny<Stream>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Upload_WithEmptyFile_ShouldReturnBadRequest()
    {
        // Arrange
        var examId = 3;
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);

        // Act
        var result = await _controller.Upload(examId, mockFile.Object, _cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is required", badRequestResult.Value);
        
        // Verify the service was not called
        _mockStorageService.Verify(x => x.UploadExamAsync(
            It.IsAny<int>(), 
            It.IsAny<Stream>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Upload_WithLargeFile_ShouldProcessSuccessfully()
    {
        // Arrange
        var examId = 4;
        var fileName = "large-scan.dcm";
        var contentType = "application/dicom";
        var largeContent = new byte[1024 * 1024]; // 1MB file
        new Random().NextBytes(largeContent);
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(largeContent.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(largeContent));

        var expectedUrl = $"https://storage.example.com/exams/{examId}/{fileName}";
        
        _mockStorageService
            .Setup(x => x.UploadExamAsync(examId, It.IsAny<Stream>(), fileName, contentType, _cancellationToken))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _controller.Upload(examId, mockFile.Object, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        var urlProperty = response!.GetType().GetProperty("Url");
        var actualUrl = urlProperty!.GetValue(response)?.ToString();
        Assert.Equal(expectedUrl, actualUrl);
    }

    [Fact]
    public async Task Upload_WhenServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var examId = 5;
        var fileName = "test.jpg";
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.Length).Returns(100);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[100]));

        _mockStorageService
            .Setup(x => x.UploadExamAsync(It.IsAny<int>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Storage service error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _controller.Upload(examId, mockFile.Object, _cancellationToken));
    }

    [Theory]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("scan.dcm", "application/dicom")]
    [InlineData("data.xml", "application/xml")]
    public async Task Upload_WithDifferentFileTypes_ShouldProcessSuccessfully(string fileName, string contentType)
    {
        // Arrange
        var examId = 6;
        var fileContent = new byte[1024];
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(fileContent.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));

        var expectedUrl = $"https://storage.example.com/exams/{examId}/{fileName}";
        
        _mockStorageService
            .Setup(x => x.UploadExamAsync(examId, It.IsAny<Stream>(), fileName, contentType, _cancellationToken))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _controller.Upload(examId, mockFile.Object, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        var urlProperty = response!.GetType().GetProperty("Url");
        var actualUrl = urlProperty!.GetValue(response)?.ToString();
        Assert.Equal(expectedUrl, actualUrl);
        
        _mockStorageService.Verify(x => x.UploadExamAsync(
            examId, 
            It.IsAny<Stream>(), 
            fileName, 
            contentType, 
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Download_WithValidParameters_ShouldReturnFileResult()
    {
        // Arrange
        var examId = 7;
        var fileName = "test-image.jpg";
        var fileContent = Encoding.UTF8.GetBytes("fake file content");
        var fileStream = new MemoryStream(fileContent);
        
        _mockStorageService
            .Setup(x => x.DownloadExamAsync(examId, fileName, _cancellationToken))
            .ReturnsAsync(fileStream);

        // Act
        var result = await _controller.Download(examId, fileName, _cancellationToken);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(fileStream, fileResult.FileStream);
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        Assert.Equal(fileName, fileResult.FileDownloadName);
        
        _mockStorageService.Verify(x => x.DownloadExamAsync(examId, fileName, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Download_WhenFileNotFound_ShouldPropagateException()
    {
        // Arrange
        var examId = 8;
        var fileName = "nonexistent.jpg";
        
        _mockStorageService
            .Setup(x => x.DownloadExamAsync(examId, fileName, _cancellationToken))
            .ThrowsAsync(new FileNotFoundException("File not found"));

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _controller.Download(examId, fileName, _cancellationToken));
        
        _mockStorageService.Verify(x => x.DownloadExamAsync(examId, fileName, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Download_WhenServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var examId = 9;
        var fileName = "test.jpg";
        
        _mockStorageService
            .Setup(x => x.DownloadExamAsync(examId, fileName, _cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _controller.Download(examId, fileName, _cancellationToken));
    }

    [Theory]
    [InlineData("image.jpg")]
    [InlineData("document.pdf")]
    [InlineData("scan.dcm")]
    [InlineData("file with spaces.txt")]
    [InlineData("file-with-dashes.xml")]
    public async Task Download_WithDifferentFileNames_ShouldReturnCorrectFileName(string fileName)
    {
        // Arrange
        var examId = 10;
        var fileContent = new byte[1024];
        var fileStream = new MemoryStream(fileContent);
        
        _mockStorageService
            .Setup(x => x.DownloadExamAsync(examId, fileName, _cancellationToken))
            .ReturnsAsync(fileStream);

        // Act
        var result = await _controller.Download(examId, fileName, _cancellationToken);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(fileName, fileResult.FileDownloadName);
        
        _mockStorageService.Verify(x => x.DownloadExamAsync(examId, fileName, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Download_WithEmptyStream_ShouldReturnEmptyFile()
    {
        // Arrange
        var examId = 11;
        var fileName = "empty.txt";
        var emptyStream = new MemoryStream();
        
        _mockStorageService
            .Setup(x => x.DownloadExamAsync(examId, fileName, _cancellationToken))
            .ReturnsAsync(emptyStream);

        // Act
        var result = await _controller.Download(examId, fileName, _cancellationToken);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal(emptyStream, fileResult.FileStream);
        Assert.Equal(0, fileResult.FileStream.Length);
    }
}