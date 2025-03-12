using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Utilities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.API.Controllers;

public class SessionsControllerTests
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly SessionsController _controller;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public SessionsControllerTests()
    {
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _controller = new SessionsController(_mockEmployeeService.Object, _mockJwtTokenService.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        // Устранение рекурсивного создания объектов
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.LoginAsync(null, _cancellationToken);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenEmployeeNotFound()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>(); // Генерируем случайный запрос
        _mockEmployeeService.Setup(service => service.GetEmployeeByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _controller.LoginAsync(request, _cancellationToken);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid email or password.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var employee = _fixture.Build<Employee>()
                               .With(employee => employee.Email, request.Email)
                               .With(employee => employee.Password, PasswordHelper.HashPassword("correctpassword"))
                               .Create();

        _mockEmployeeService.Setup(service => service.GetEmployeeByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

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
        var employee = _fixture.Build<Employee>()
                               .With(employee => employee.Email, request.Email)
                               .With(employee => employee.Password, PasswordHelper.HashPassword(request.Password))
                               .Create();
        var token = _fixture.Create<string>();

        _mockEmployeeService.Setup(service => service.GetEmployeeByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _mockJwtTokenService.Setup(service => service.GenerateToken(employee))
            .Returns(token);

        // Act
        var result = await _controller.LoginAsync(request, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal(token, response.Token);
    }
}
