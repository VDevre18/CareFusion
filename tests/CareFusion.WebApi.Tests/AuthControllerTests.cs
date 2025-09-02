using CareFusion.Business.Services;
using CareFusion.Model.Dtos;
using CareFusion.Model.Responses;
using CareFusion.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CareFusion.WebApi.Tests;

public class AuthControllerTests
{
    private readonly Mock<AuthManager> _mockAuthManager;
    private readonly AuthController _controller;
    private readonly CancellationToken _cancellationToken;

    public AuthControllerTests()
    {
        var mockUow = new Mock<CareFusion.Core.Repositories.Interfaces.IUnitOfWork>();
        _mockAuthManager = new Mock<AuthManager>(mockUow.Object);
        _controller = new AuthController(_mockAuthManager.Object);
        _cancellationToken = CancellationToken.None;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccessResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("admin@carefusion.com", "admin123");
        var expectedUser = new UserDto
        {
            Id = 1,
            Username = "admin@carefusion.com",
            Email = "admin@carefusion.com",
            IsActive = true
        };
        
        var expectedResponse = ApiResponse<UserDto>.Ok(expectedUser);
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(expectedUser.Username, response.Data!.Username);
        Assert.Equal(expectedUser.Email, response.Data.Email);
        Assert.True(response.Data.IsActive);
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ShouldReturnUnauthorizedResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("invalid@carefusion.com", "admin123");
        var expectedResponse = ApiResponse<UserDto>.Fail("Invalid username or password");
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Invalid username or password", response.Message);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorizedResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("admin@carefusion.com", "wrongpassword");
        var expectedResponse = ApiResponse<UserDto>.Fail("Invalid username or password");
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Invalid username or password", response.Message);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ShouldReturnBadRequestResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("", "admin123");
        var expectedResponse = ApiResponse<UserDto>.Fail("Username is required");
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Username is required", response.Message);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequestResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("admin@carefusion.com", "");
        var expectedResponse = ApiResponse<UserDto>.Fail("Password is required");
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Password is required", response.Message);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ShouldReturnUnauthorizedResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest("inactive@carefusion.com", "admin123");
        var expectedResponse = ApiResponse<UserDto>.Fail("User account is inactive");
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User account is inactive", response.Message);
    }

    [Fact]
    public async Task Login_ShouldCallAuthManagerWithCorrectParameters()
    {
        // Arrange
        var loginRequest = new LoginRequest("test@carefusion.com", "testpass123");
        var expectedResponse = ApiResponse<UserDto>.Ok(new UserDto 
        { 
            Id = 2, 
            Username = "test@carefusion.com", 
            Email = "test@carefusion.com",
            IsActive = true
        });
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(loginRequest.Username, loginRequest.Password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        _mockAuthManager.Verify(x => x.LoginAsync(
            "test@carefusion.com", 
            "testpass123", 
            _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData("admin@carefusion.com", "admin123")]
    [InlineData("user@carefusion.com", "user123")]
    [InlineData("test.user@carefusion.com", "password")]
    public async Task Login_WithVariousValidCredentials_ShouldCallAuthManager(string username, string password)
    {
        // Arrange
        var loginRequest = new LoginRequest(username, password);
        var expectedResponse = ApiResponse<UserDto>.Ok(new UserDto 
        { 
            Id = 3, 
            Username = username, 
            Email = username,
            IsActive = true
        });
        
        _mockAuthManager
            .Setup(x => x.LoginAsync(username, password, _cancellationToken))
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.Login(loginRequest, _cancellationToken);

        // Assert
        _mockAuthManager.Verify(x => x.LoginAsync(username, password, _cancellationToken), Times.Once);
    }
}