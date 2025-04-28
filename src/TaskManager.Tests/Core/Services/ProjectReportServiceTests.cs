using AutoFixture;
using Moq;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class ProjectReportServiceTests
{
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly ProjectReportService _service;
    private readonly CancellationToken _cancellationToken;
    private readonly Fixture _fixture;

    public ProjectReportServiceTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _service = new ProjectReportService(_mockTaskService.Object);
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GenerateProjectReportAsync_ReturnsReport_WhenTasksExist()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var expectedTitle = "Test Project";

        var tasks = _fixture.Build<TaskManager.Core.Models.Task>()
            .With(task => task.Project, new Project { Title = expectedTitle })
            .CreateMany(5)
            .ToList();

        _mockTaskService.Setup(tasks => tasks.GetTasksByProjectIdAsync(projectId, _cancellationToken))
            .ReturnsAsync(tasks);

        // Act
        var report = await _service.GenerateProjectReportAsync(projectId, _cancellationToken);

        // Assert
        Assert.Contains($"<h1>Отчет по проекту: {expectedTitle}</h1>", report);
        Assert.Contains("<table border='1'>", report);
        Assert.Contains(tasks.First().Title, report);
    }

    [Fact]
    public async Task GenerateProjectReportAsync_ReturnsNoTasksMessage_WhenNoTasksExist()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();

        _mockTaskService.Setup(tasks => tasks.GetTasksByProjectIdAsync(projectId, _cancellationToken))
            .ReturnsAsync(new List<TaskManager.Core.Models.Task>());

        // Act
        var report = await _service.GenerateProjectReportAsync(projectId, _cancellationToken);

        // Assert
        Assert.Contains($"<h1>Отчет по проекту {projectId}</h1>", report);
        Assert.Contains("Нет задач для данного проекта.", report);
    }
}