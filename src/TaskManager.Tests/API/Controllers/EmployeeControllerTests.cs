using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Tests.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _mockEmployeeService;
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly EmployeeController _controller;
        private readonly Fixture _fixture;
        private readonly CancellationToken _cancellationToken;

        public EmployeeControllerTests()
        {
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockProjectService = new Mock<IProjectService>();
            _mockTaskService = new Mock<ITaskService>();
            _controller = new EmployeeController(_mockEmployeeService.Object, _mockProjectService.Object, _mockTaskService.Object);
            _fixture = new Fixture();
            _cancellationToken = new CancellationToken();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsOkResult_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employee = _fixture.Build<Employee>().With(employee => employee.Id, employeeId).Create();
            _mockEmployeeService.Setup(service => service.GetByIdAsync(employeeId, _cancellationToken))
                                .ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployeeByIdAsync(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var employeeResponse = Assert.IsType<EmployeeResponse>(okResult.Value);
            Assert.Equal(employeeId, employeeResponse.Id);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsNotFoundResult_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockEmployeeService.Setup(service => service.GetByIdAsync(employeeId, _cancellationToken))
                                .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.GetEmployeeByIdAsync(employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddEmployeeAsync_ReturnsCreatedAtActionResult_WhenEmployeeIsCreated()
        {
            // Arrange
            var employeeRequest = _fixture.Create<EmployeeRequest>();
            var createdEmployee = _fixture.Build<Employee>()
                                          .With(employee => employee.Name, employeeRequest.Name)
                                          .With(employee => employee.Email, employeeRequest.Email)
                                          .Create();
            _mockEmployeeService.Setup(service => service.AddAsync(It.IsAny<Employee>(), _cancellationToken))
                                .ReturnsAsync(createdEmployee);

            // Act
            var result = await _controller.AddEmployeeAsync(employeeRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var employeeResponse = Assert.IsType<EmployeeResponse>(createdAtActionResult.Value);
            Assert.Equal(createdEmployee.Id, employeeResponse.Id);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ReturnsNoContentResult_WhenEmployeeIsUpdated()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employeeRequest = _fixture.Create<EmployeeRequest>();
            _mockEmployeeService.Setup(service => service.UpdateAsync(It.IsAny<Employee>(), _cancellationToken))
                                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(employeeId, employeeRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ReturnsNotFoundResult_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employeeRequest = _fixture.Create<EmployeeRequest>();
            _mockEmployeeService.Setup(service => service.UpdateAsync(It.IsAny<Employee>(), _cancellationToken))
                                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAsync(employeeId, employeeRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ReturnsNoContentResult_WhenEmployeeIsDeleted()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _mockEmployeeService.Setup(service => service.DeleteAsync(employeeId, _cancellationToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteEmployeeAsync(employeeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetProjectsByEmployeeIdAsync_ReturnsOkResult_WithProjects()
        {
            // Arrange
            var expectedCount = 2;
            var employeeId = Guid.NewGuid();
            var projects = _fixture.CreateMany<Project>(expectedCount).ToList();
            _mockProjectService.Setup(service => service.GetProjectsByEmployeeIdAsync(employeeId, _cancellationToken))
                               .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetProjectsByEmployeeIdAsync(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var projectResponses = Assert.IsType<List<ProjectResponse>>(okResult.Value);
            Assert.Equal(expectedCount, projectResponses.Count);
        }

        [Fact]
        public async Task GetTasksByEmployeeIdAsync_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var expectedCount = 2;
            var employeeId = Guid.NewGuid();
            var tasks = _fixture.CreateMany<Core.Models.Task>(expectedCount).ToList();
            _mockTaskService.Setup(service => service.GetTasksByEmployeeIdAsync(employeeId, _cancellationToken))
                            .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetTasksByEmployeeIdAsync(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var taskResponses = Assert.IsType<List<TaskResponse>>(okResult.Value);
            Assert.Equal(expectedCount, taskResponses.Count);
        }

        [Fact]
        public async Task ListAllEmployeesAsync_ReturnsOkResult_WithEmployees()
        {
            // Arrange
            var expectedCount = 2;
            var employees = _fixture.CreateMany<Employee>(expectedCount).ToList();
            _mockEmployeeService.Setup(service => service.ListAllAsync(_cancellationToken))
                                .ReturnsAsync(employees);

            // Act
            var result = await _controller.ListAllEmployeesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var employeeResponses = Assert.IsType<List<EmployeeResponse>>(okResult.Value);
            Assert.Equal(expectedCount, employeeResponses.Count);
        }
    }
}
