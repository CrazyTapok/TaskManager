using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Tests.API.Controllers;

public class SessionControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly SessionController _controller;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public SessionControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new SessionController(_mockAuthService.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        // Eliminate recursive behaviors for object generation
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }


    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenAuthenticationFails()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        _mockAuthService.Setup(service => service.AuthenticateAsync(request.Email, request.Password, _cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

        // Act
        var result = await _controller.LoginAsync(request, _cancellationToken);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid email or password.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var token = _fixture.Create<string>();

        _mockAuthService.Setup(service => service.AuthenticateAsync(request.Email, request.Password, _cancellationToken))
            .ReturnsAsync(token);

        // Act
        var result = await _controller.LoginAsync(request, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal(token, response.Token);
    }
}
