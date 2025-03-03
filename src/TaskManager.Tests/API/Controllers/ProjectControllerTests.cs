using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly ProjectController _controller;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public ProjectControllerTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _mockTaskService = new Mock<ITaskService>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        _controller = new ProjectController(_mockProjectService.Object, _mockEmployeeService.Object, _mockTaskService.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsOkResult_WhenProjectExists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = _fixture.Build<Project>().With(project => project.Id, projectId).Create();
        _mockProjectService.Setup(service => service.GetByIdAsync(projectId, _cancellationToken))
                           .ReturnsAsync(project);

        // Act
        var result = await _controller.GetProjectByIdAsync(projectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var projectResponse = Assert.IsType<ProjectResponse>(okResult.Value);
        Assert.Equal(projectId, projectResponse.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsNotFoundResult_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _mockProjectService.Setup(service => service.GetByIdAsync(projectId, _cancellationToken))
                           .ReturnsAsync((Project)null);

        // Act
        var result = await _controller.GetProjectByIdAsync(projectId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddProjectAsync_ReturnsCreatedAtActionResult_WhenProjectIsCreated()
    {
        // Arrange
        var projectRequest = _fixture.Create<ProjectRequest>();
        var createdProject = _fixture.Build<Project>().With(project => project.Title, projectRequest.Title).Create();
        _mockProjectService.Setup(service => service.AddAsync(It.IsAny<Project>(), _cancellationToken))
                           .ReturnsAsync(createdProject);

        // Act
        var result = await _controller.AddProjectAsync(projectRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var projectResponse = Assert.IsType<ProjectResponse>(createdAtActionResult.Value);
        Assert.Equal(createdProject.Id, projectResponse.Id);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsNoContentResult_WhenProjectIsUpdated()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectRequest = _fixture.Create<ProjectRequest>();
        _mockProjectService.Setup(service => service.UpdateAsync(It.IsAny<Project>(), _cancellationToken))
                           .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateProjectAsync(projectId, projectRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsNotFoundResult_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectRequest = _fixture.Create<ProjectRequest>();
        _mockProjectService.Setup(service => service.UpdateAsync(It.IsAny<Project>(), _cancellationToken))
                           .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateProjectAsync(projectId, projectRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_ReturnsNoContentResult_WhenProjectIsDeleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _mockProjectService.Setup(service => service.DeleteAsync(projectId, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProjectAsync(projectId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetTasksByProjectIdAsync_ReturnsOkResult_WithTasks()
    {
        // Arrange
        var expectedCount = 2;
        var projectId = Guid.NewGuid();
        var tasks = _fixture.CreateMany<Core.Models.Task>(expectedCount).ToList();
        _mockTaskService.Setup(service => service.GetTasksByProjectIdAsync(projectId, _cancellationToken))
                        .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasksByProjectIdAsync(projectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var taskResponses = Assert.IsType<List<TaskResponse>>(okResult.Value);
        Assert.Equal(expectedCount, taskResponses.Count);
    }

    [Fact]
    public async Task GetEmployeesByProjectIdAsync_ReturnsOkResult_WithEmployees()
    {
        // Arrange
        var expectedCount = 2;
        var projectId = Guid.NewGuid();
        var employees = _fixture.CreateMany<Employee>(expectedCount).ToList();
        _mockEmployeeService.Setup(service => service.GetEmployeesByProjectIdAsync(projectId, _cancellationToken))
                            .ReturnsAsync(employees);

        // Act
        var result = await _controller.GetEmployeesByProjectIdAsync(projectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var employeeResponses = Assert.IsType<List<EmployeeResponse>>(okResult.Value);
        Assert.Equal(expectedCount, employeeResponses.Count);
    }

    [Fact]
    public async Task ListAllProjectsAsync_ReturnsOkResult_WithProjects()
    {
        // Arrange
        var expectedCount = 2;
        var projects = _fixture.CreateMany<Project>(expectedCount).ToList();
        _mockProjectService.Setup(service => service.ListAllAsync(_cancellationToken))
                           .ReturnsAsync(projects);

        // Act
        var result = await _controller.ListAllProjectsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var projectResponses = Assert.IsType<List<ProjectResponse>>(okResult.Value);
        Assert.Equal(expectedCount, projectResponses.Count);
    }
}