using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace CareFusion.WebApi.Tests;

public class ExamsControllerTests
{
    private readonly Mock<ExamManager> _mockExamManager;
    private readonly ExamsController _controller;
    private readonly CancellationToken _cancellationToken;

    public ExamsControllerTests()
    {
        var mockUow = new Mock<CareFusion.Core.Repositories.Interfaces.IUnitOfWork>();
        var mockMapper = new Mock<AutoMapper.IMapper>();
        var mockValidator = new Mock<FluentValidation.IValidator<ExamDto>>();
        _mockExamManager = new Mock<ExamManager>(mockUow.Object, mockMapper.Object, mockValidator.Object);
        _controller = new ExamsController(_mockExamManager.Object);
        _cancellationToken = CancellationToken.None;

        // Setup user context
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "test@example.com"),
            new(ClaimTypes.NameIdentifier, "test-user-id")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    [Fact]
    public async Task Get_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var examId = 1;
        var patientId = 2;
        var expectedResponse = ApiResponse<ExamDto>.Ok(new ExamDto
        {
            Id = examId,
            PatientId = patientId,
            Modality = "CT",
            StudyType = "Chest CT",
            StudyDateUtc = DateTime.UtcNow.AddDays(-1),
            Status = "Completed"
        });
        
        _mockExamManager
            .Setup(x => x.GetByIdAsync(examId, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get(examId, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(examId, response.Data!.Id);
        Assert.Equal("CT", response.Data.Modality);
        Assert.Equal("Chest CT", response.Data.StudyType);
    }

    [Fact]
    public async Task Get_WithInvalidId_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var examId = 3;
        var expectedResponse = ApiResponse<ExamDto>.Fail("Exam not found");
        
        _mockExamManager
            .Setup(x => x.GetByIdAsync(examId, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get(examId, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Exam not found", response.Message);
    }

    [Fact]
    public async Task ListByPatient_WithValidPatientId_ShouldReturnPagedResults()
    {
        // Arrange
        var patientId = 4;
        const int page = 1;
        const int pageSize = 20;
        
        var exams = new List<ExamDto>
        {
            new() 
            { 
                Id = 5, 
                PatientId = patientId, 
                Modality = "CT", 
                StudyType = "Chest CT",
                StudyDateUtc = DateTime.UtcNow.AddDays(-2),
                Status = "Completed"
            },
            new() 
            { 
                Id = 6, 
                PatientId = patientId, 
                Modality = "MRI", 
                StudyType = "Brain MRI",
                StudyDateUtc = DateTime.UtcNow.AddDays(-1),
                Status = "In Progress"
            }
        };
        
        var pagedResult = new PagedResult<ExamDto>
        {
            Items = exams,
            TotalCount = 2,
            Page = page,
            PageSize = pageSize
        };
        
        var expectedResponse = ApiResponse<PagedResult<ExamDto>>.Ok(pagedResult);
        
        _mockExamManager
            .Setup(x => x.ListByPatientAsync(patientId, page, pageSize, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ListByPatient(patientId, page, pageSize, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<ExamDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Items.Count);
        Assert.Equal(2, response.Data.TotalCount);
        Assert.All(response.Data.Items, exam => Assert.Equal(patientId, exam.PatientId));
    }

    [Fact]
    public async Task ListByPatient_WithNonExistentPatient_ShouldReturnEmptyResults()
    {
        // Arrange
        var patientId = 7;
        const int page = 1;
        const int pageSize = 20;
        
        var pagedResult = new PagedResult<ExamDto>
        {
            Items = new List<ExamDto>(),
            TotalCount = 0,
            Page = page,
            PageSize = pageSize
        };
        
        var expectedResponse = ApiResponse<PagedResult<ExamDto>>.Ok(pagedResult);
        
        _mockExamManager
            .Setup(x => x.ListByPatientAsync(patientId, page, pageSize, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ListByPatient(patientId, page, pageSize, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<ExamDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Data!.Items);
        Assert.Equal(0, response.Data.TotalCount);
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedExam()
    {
        // Arrange
        var patientId = 8;
        var examDto = new ExamDto
        {
            PatientId = patientId,
            Modality = "MRI",
            StudyType = "Lumbar Spine MRI",
            StudyDateUtc = DateTime.UtcNow,
            Status = "Scheduled"
        };
        
        var createdExamDto = examDto with { Id = 9 };
        var expectedResponse = ApiResponse<ExamDto>.Ok(createdExamDto);
        
        _mockExamManager
            .Setup(x => x.CreateAsync(examDto, "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(examDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotEqual(0, response.Data!.Id);
        Assert.Equal("MRI", response.Data.Modality);
        Assert.Equal("Lumbar Spine MRI", response.Data.StudyType);
    }

    [Fact]
    public async Task Create_WithInvalidDto_ShouldReturnValidationError()
    {
        // Arrange
        var invalidExamDto = new ExamDto
        {
            PatientId = 0, // Invalid - empty patient ID
            Modality = "CT",
            StudyType = "Invalid Study",
            Status = "Scheduled"
        };
        
        var expectedResponse = ApiResponse<ExamDto>.Fail("Patient ID is required");
        
        _mockExamManager
            .Setup(x => x.CreateAsync(invalidExamDto, "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(invalidExamDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Patient ID is required", response.Message);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnUpdatedExam()
    {
        // Arrange
        var examId = 10;
        var patientId = 11;
        var examDto = new ExamDto
        {
            Id = examId,
            PatientId = patientId,
            Modality = "CT",
            StudyType = "Updated Study Type",
            StudyDateUtc = DateTime.UtcNow,
            Status = "In Progress"
        };
        
        var expectedResponse = ApiResponse<ExamDto>.Ok(examDto);
        
        _mockExamManager
            .Setup(x => x.UpdateAsync(It.Is<ExamDto>(e => e.Id == examId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(examId, examDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(examId, response.Data!.Id);
        Assert.Equal("Updated Study Type", response.Data.StudyType);
        Assert.Equal("In Progress", response.Data.Status);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var examId = 12;
        var patientId = 13;
        var examDto = new ExamDto
        {
            Id = examId,
            PatientId = patientId,
            Modality = "CT",
            StudyType = "Non-existent Study",
            Status = "Completed"
        };
        
        var expectedResponse = ApiResponse<ExamDto>.Fail("Exam not found");
        
        _mockExamManager
            .Setup(x => x.UpdateAsync(It.Is<ExamDto>(e => e.Id == examId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(examId, examDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Exam not found", response.Message);
    }

    [Fact]
    public async Task Update_ShouldOverrideIdFromRoute()
    {
        // Arrange
        var routeId = 14;
        var dtoId = 15; // Different ID in DTO
        var patientId = 16;
        
        var examDto = new ExamDto
        {
            Id = dtoId, // This should be ignored
            PatientId = patientId,
            Modality = "MRI",
            StudyType = "Test Study",
            Status = "New"
        };
        
        var expectedResponse = ApiResponse<ExamDto>.Ok(examDto with { Id = routeId });
        
        _mockExamManager
            .Setup(x => x.UpdateAsync(It.Is<ExamDto>(e => e.Id == routeId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(routeId, examDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<ExamDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(routeId, response.Data!.Id); // Should use route ID, not DTO ID
        
        // Verify the manager was called with the correct ID
        _mockExamManager.Verify(x => x.UpdateAsync(
            It.Is<ExamDto>(e => e.Id == routeId), 
            "test@example.com", 
            _cancellationToken), Times.Once);
    }
}