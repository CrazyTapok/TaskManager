using AutoFixture;
using Moq;
using TaskManager.Core.Interfaces.Services.Authentication;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Interfaces.Services.Security;
using TaskManager.Core.Models;
using TaskManager.Core.Services.Authentication;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services.AuthenticationTests;

public class AuthServiceTests
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly AuthService _authService;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public AuthServiceTests()
    {
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _authService = new AuthService(_mockEmployeeService.Object, _mockJwtTokenService.Object, _mockPasswordHasher.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        // Убираем рекурсивные зависимости, чтобы избежать ошибок в генерации
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var hashedPassword = _fixture.Create<string>(); 

        var employee = _fixture.Build<Employee>()
            .With(employee => employee.Password, hashedPassword)
            .Create();

        _mockEmployeeService
            .Setup(service => service.GetEmployeeByEmailAsync(employee.Email, _cancellationToken))
            .ReturnsAsync(employee);

        _mockPasswordHasher
            .Setup(passwordHasher => passwordHasher.VerifyPassword(password, hashedPassword))
            .Returns(true);

        var expectedToken = _fixture.Create<string>();
        _mockJwtTokenService
            .Setup(service => service.GenerateToken(employee))
            .Returns(expectedToken);

        // Act
        var token = await _authService.AuthenticateAsync(employee.Email, password, _cancellationToken);

        // Assert
        Assert.Equal(expectedToken, token);
        _mockEmployeeService.Verify(service => service.GetEmployeeByEmailAsync(employee.Email, _cancellationToken), Times.Once);
        _mockJwtTokenService.Verify(service => service.GenerateToken(employee), Times.Once);
        _mockPasswordHasher.Verify(passwordHasher => passwordHasher.VerifyPassword(password, hashedPassword), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ThrowsUnauthorizedAccessException_WhenEmployeeNotFound()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _mockEmployeeService.Setup(service => service.GetEmployeeByEmailAsync(email, _cancellationToken))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.AuthenticateAsync(email, password, _cancellationToken));

        Assert.Equal("Invalid email or password.", exception.Message);
        _mockEmployeeService.Verify(service => service.GetEmployeeByEmailAsync(email, _cancellationToken), Times.Once);
        _mockJwtTokenService.Verify(service => service.GenerateToken(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_ThrowsUnauthorizedAccessException_WhenPasswordIsInvalid()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var hashedPassword = _fixture.Create<string>();

        var employee = _fixture.Build<Employee>()
            .With(e => e.Password, hashedPassword)
            .Create();

        _mockEmployeeService
            .Setup(service => service.GetEmployeeByEmailAsync(email, _cancellationToken))
            .ReturnsAsync(employee);

        _mockPasswordHasher
            .Setup(h => h.VerifyPassword(password, hashedPassword))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.AuthenticateAsync(email, password, _cancellationToken));

        Assert.Equal("Invalid email or password.", exception.Message);

        _mockEmployeeService.Verify(service => service.GetEmployeeByEmailAsync(email, _cancellationToken), Times.Once);
        _mockJwtTokenService.Verify(service => service.GenerateToken(It.IsAny<Employee>()), Times.Never);
        _mockPasswordHasher.Verify(h => h.VerifyPassword(password, hashedPassword), Times.Once);
    }
}
