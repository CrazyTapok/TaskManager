using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Core.Utilities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services
{
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
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => employees.Where(predicate.Compile()).ToList());

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
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => employees.Where(predicate.Compile()).ToList());

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

        [Fact]
        public async Task GetEmployeeByEmailAsync_ThrowsArgumentException_WhenEmailIsNullOrWhitespace()
        {
            // Arrange
            var invalidEmails = new[] { null, "", " " };

            foreach (var email in invalidEmails)
            {
                // Act & Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                    _service.GetEmployeeByEmailAsync(email!, _cancellationToken));

                Assert.Equal("Email cannot be null or whitespace. (Parameter 'email')", exception.Message);
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
                .With(e => e.Email, "test007@example.com")
                .With(e => e.Password, originalPassword)
                .Create();

            _mockRepo.Setup(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken))
                .ReturnsAsync((Employee?)null);

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Employee>(), _cancellationToken))
                .ReturnsAsync((Employee e, CancellationToken _) => e);

            // Act
            var result = await _service.RegisterAsync(employee, _cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employee.Email, result.Email);
            Assert.NotEqual(originalPassword, result.Password);

            // Assuming `PasswordHelper` has a `VerifyPassword` method:
            Assert.True(PasswordHelper.VerifyPassword(originalPassword, result.Password));

            _mockRepo.Verify(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken), Times.Once);
            _mockRepo.Verify(repo => repo.AddAsync(It.Is<Employee>(e => e.Password != originalPassword && PasswordHelper.VerifyPassword(originalPassword, e.Password)), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsInvalidOperationException_WhenEmployeeAlreadyExists()
        {
            // Arrange
            var employee = _fixture.Build<Employee>().With(e => e.Email, "test007@example.com").Create();
            _mockRepo.Setup(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken))
                .ReturnsAsync(employee);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RegisterAsync(employee, _cancellationToken));

            Assert.Equal("A user with this email already exists.", exception.Message);
            _mockRepo.Verify(repo => repo.GetByEmailAsync(employee.Email, _cancellationToken), Times.Once);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Employee>(), _cancellationToken), Times.Never);
        }
    }
}
