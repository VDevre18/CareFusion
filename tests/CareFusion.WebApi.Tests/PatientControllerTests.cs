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

public class PatientsControllerTests
{
    private readonly Mock<PatientManager> _mockPatientManager;
    private readonly PatientsController _controller;
    private readonly CancellationToken _cancellationToken;

    public PatientsControllerTests()
    {
        var mockUow = new Mock<CareFusion.Core.Repositories.Interfaces.IUnitOfWork>();
        var mockMapper = new Mock<AutoMapper.IMapper>();
        var mockValidator = new Mock<FluentValidation.IValidator<PatientDto>>();
        _mockPatientManager = new Mock<PatientManager>(mockUow.Object, mockMapper.Object, mockValidator.Object);
        _controller = new PatientsController(_mockPatientManager.Object);
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
        var patientId = Guid.NewGuid();
        var expectedResponse = ApiResponse<PatientDto>.Ok(new PatientDto 
        { 
            Id = patientId,
            FirstName = "John",
            LastName = "Doe",
            MRN = "TEST001",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = "Male"
        });
        
        _mockPatientManager
            .Setup(x => x.GetByIdAsync(patientId, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get(patientId, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(patientId, response.Data!.Id);
        Assert.Equal("John", response.Data.FirstName);
    }

    [Fact]
    public async Task Get_WithInvalidId_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var expectedResponse = ApiResponse<PatientDto>.Fail("Patient not found");
        
        _mockPatientManager
            .Setup(x => x.GetByIdAsync(patientId, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get(patientId, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Patient not found", response.Message);
    }

    [Fact]
    public async Task Search_WithTerm_ShouldReturnPagedResults()
    {
        // Arrange
        const string searchTerm = "John";
        const int page = 1;
        const int pageSize = 20;
        
        var patients = new List<PatientDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", MRN = "TEST001" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Johnson", MRN = "TEST002" }
        };
        
        var pagedResult = new PagedResult<PatientDto>
        {
            Items = patients,
            TotalCount = 2,
            Page = page,
            PageSize = pageSize
        };
        
        var expectedResponse = ApiResponse<PagedResult<PatientDto>>.Ok(pagedResult);
        
        _mockPatientManager
            .Setup(x => x.SearchAsync(searchTerm, page, pageSize, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Search(searchTerm, page, pageSize, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<PatientDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Items.Count);
        Assert.Equal(2, response.Data.TotalCount);
    }

    [Fact]
    public async Task Search_WithoutTerm_ShouldReturnAllResults()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 20;
        
        var patients = new List<PatientDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", MRN = "TEST001" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", MRN = "TEST002" },
            new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", MRN = "TEST003" }
        };
        
        var pagedResult = new PagedResult<PatientDto>
        {
            Items = patients,
            TotalCount = 3,
            Page = page,
            PageSize = pageSize
        };
        
        var expectedResponse = ApiResponse<PagedResult<PatientDto>>.Ok(pagedResult);
        
        _mockPatientManager
            .Setup(x => x.SearchAsync(null, page, pageSize, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Search(null, page, pageSize, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<PatientDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(3, response.Data!.Items.Count);
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedPatient()
    {
        // Arrange
        var patientDto = new PatientDto
        {
            FirstName = "New",
            LastName = "Patient",
            MRN = "NEW001",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Female"
        };
        
        var createdPatientDto = patientDto with { Id = Guid.NewGuid() };
        var expectedResponse = ApiResponse<PatientDto>.Ok(createdPatientDto);
        
        _mockPatientManager
            .Setup(x => x.CreateAsync(patientDto, "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(patientDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotEqual(Guid.Empty, response.Data!.Id);
        Assert.Equal("New", response.Data.FirstName);
    }

    [Fact]
    public async Task Create_WithInvalidDto_ShouldReturnValidationError()
    {
        // Arrange
        var invalidPatientDto = new PatientDto
        {
            FirstName = "", // Invalid - empty first name
            LastName = "Patient",
            MRN = "INVALID001"
        };
        
        var expectedResponse = ApiResponse<PatientDto>.Fail("First name is required");
        
        _mockPatientManager
            .Setup(x => x.CreateAsync(invalidPatientDto, "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(invalidPatientDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("First name is required", response.Message);
    }

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnUpdatedPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patientDto = new PatientDto
        {
            Id = patientId,
            FirstName = "Updated",
            LastName = "Patient",
            MRN = "UPDATE001",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = "Male"
        };
        
        var expectedResponse = ApiResponse<PatientDto>.Ok(patientDto);
        
        _mockPatientManager
            .Setup(x => x.UpdateAsync(It.Is<PatientDto>(p => p.Id == patientId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(patientId, patientDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(patientId, response.Data!.Id);
        Assert.Equal("Updated", response.Data.FirstName);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patientDto = new PatientDto
        {
            Id = patientId,
            FirstName = "NonExistent",
            LastName = "Patient",
            MRN = "NOTFOUND001"
        };
        
        var expectedResponse = ApiResponse<PatientDto>.Fail("Patient not found");
        
        _mockPatientManager
            .Setup(x => x.UpdateAsync(It.Is<PatientDto>(p => p.Id == patientId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(patientId, patientDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Patient not found", response.Message);
    }

    [Fact]
    public async Task Update_ShouldOverrideIdFromRoute()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var dtoId = Guid.NewGuid(); // Different ID in DTO
        
        var patientDto = new PatientDto
        {
            Id = dtoId, // This should be ignored
            FirstName = "Test",
            LastName = "Patient",
            MRN = "OVERRIDE001"
        };
        
        var expectedResponse = ApiResponse<PatientDto>.Ok(patientDto with { Id = routeId });
        
        _mockPatientManager
            .Setup(x => x.UpdateAsync(It.Is<PatientDto>(p => p.Id == routeId), "test@example.com", _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(routeId, patientDto, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(routeId, response.Data!.Id); // Should use route ID, not DTO ID
        
        // Verify the manager was called with the correct ID
        _mockPatientManager.Verify(x => x.UpdateAsync(
            It.Is<PatientDto>(p => p.Id == routeId), 
            "test@example.com", 
            _cancellationToken), Times.Once);
    }
}