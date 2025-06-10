using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services.ProjectManagement;
using TaskManager.Core.Utilities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services.ProjectManagementTests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepo;
    private readonly EmployeeService _service;
    private readonly CancellationToken _cancellationToken;
    private readonly Fixture _fixture;

    public EmployeeServiceTests()
    {
        _mockRepo = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_mockRepo.Object);
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }


    [Fact]
    public async Task GetEmployeesByProjectIdAsync()
    {
        // Arrange
        var expectedCount = 2;
        var projectId = Guid.NewGuid();
        var employees = new List<Employee>
        {
            _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, projectId).Create()]).Create(),
            _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, projectId).Create()]).Create(),
            _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, Guid.NewGuid).Create()]).Create()
        };

        _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
            .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => [.. employees.Where(predicate.Compile())]);

        // Act
        var result = await _service.GetEmployeesByProjectIdAsync(projectId, _cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, employee => Assert.Contains(employee, employees));
    }

    [Fact]
    public async Task GetEmployeesByCompanyIdAsync()
    {
        // Arrange
        var expectedCount = 2;
        var companyId = Guid.NewGuid();
        var employees = new List<Employee>
        {

            _fixture.Build<Employee>().With(employee => employee.CompanyId, companyId).Create(),
            _fixture.Build<Employee>().With(employee => employee.CompanyId, companyId).Create(),
            _fixture.Build<Employee>().With(employee => employee.CompanyId, Guid.NewGuid).Create()
        };

        _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
            .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => [.. employees.Where(predicate.Compile())]);

        // Act
        var result = await _service.GetEmployeesByCompanyIdAsync(companyId, _cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, employee => Assert.Contains(employee, employees));
    }

    [Fact]
    public async Task GetEmployeeByEmailAsync_ReturnsEmployee_WhenEmailIsValid()
    {
        // Arrange
        var email = "test@example.com";
        var employee = _fixture.Build<Employee>().With(employee => employee.Email, email).Create();

        _mockRepo.Setup(repo => repo.GetByEmailAsync(email, _cancellationToken))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetEmployeeByEmailAsync(email, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result!.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetEmployeeByEmailAsync_ThrowsException_WhenEmailIsNullOrWhitespace(string invalidEmail)
    {
        // Act & Assert
        if (invalidEmail == null)
        {
            var nullException = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetEmployeeByEmailAsync(invalidEmail!, _cancellationToken));
            Assert.Equal("Value cannot be null. (Parameter 'email')", nullException.Message);
        }
        else
        {
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetEmployeeByEmailAsync(invalidEmail, _cancellationToken));
            Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'email')", argumentException.Message);
        }
    }

    [Fact]
    public async Task GetEmployeeByEmailAsync_ReturnsNull_WhenEmployeeNotFound()
    {
        // Arrange
        var email = "test007@example.com";

        _mockRepo.Setup(repo => repo.GetByEmailAsync(email, _cancellationToken)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetEmployeeByEmailAsync(email, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_AddsEmployee_WhenValid()
    {
        // Arrange
        var originalPassword = _fixture.Create<string>();
        var employee = _fixture.Build<Employee>()
            .With(employee => employee.Email, "test007@example.com")
            .With(employee => employee.Password, originalPassword)
            .Create();

        _mockRepo.Setup(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken))
            .ReturnsAsync((Employee?)null);

        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Employee>(), _cancellationToken))
            .ReturnsAsync((Employee employee, CancellationToken _) => employee);

        // Act
        var result = await _service.RegisterAsync(employee, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Email, result.Email);
        Assert.NotEqual(originalPassword, result.Password);

        // Assuming `PasswordHelper` has a `VerifyPassword` method:
        Assert.True(PasswordHelper.VerifyPassword(originalPassword, result.Password));

        _mockRepo.Verify(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken), Times.Once);
        _mockRepo.Verify(repo => repo.AddAsync(It.Is<Employee>(employee => employee.Password != originalPassword && PasswordHelper.VerifyPassword(originalPassword, employee.Password)), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsInvalidOperationException_WhenEmployeeAlreadyExists()
    {
        // Arrange
        var employee = _fixture.Build<Employee>().With(employee => employee.Email, "test007@example.com").Create();
        _mockRepo.Setup(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken))
            .ReturnsAsync(employee);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegisterAsync(employee, _cancellationToken));

        Assert.Equal("A user with this email already exists.", exception.Message);
        _mockRepo.Verify(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken), Times.Once);
        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Employee>(), _cancellationToken), Times.Never);
    }


    [Fact]
    public async Task GetEmployeesWithDailyNewsletterEnabledAsync_ReturnsOnlyEnabledEmployees()
    {
        // Arrange
        var expectedCount = 2;
        var employees = new List<Employee>
    {
        _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, true).Create(),
        _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, true).Create(),
        _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, false).Create()
    };

        _mockRepo.Setup(repo => repo.GetNewsletterEnabledEmployeesAsync(_cancellationToken))
            .ReturnsAsync(employees.Where(employee => employee.IsDailyNewsletterEnabled).ToList());

        // Act
        var result = await _service.GetEmployeesWithDailyNewsletterEnabledAsync(_cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, employee => Assert.True(employee.IsDailyNewsletterEnabled));
    }

}
