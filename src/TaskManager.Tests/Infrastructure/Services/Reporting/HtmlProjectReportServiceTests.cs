using AutoFixture;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Services.Reporting;

namespace TaskManager.Tests.Infrastructure.Services.Reporting;

public class HtmlProjectReportServiceTests
{
    private readonly HtmlProjectReportService _service;
    private readonly Fixture _fixture;

    public HtmlProjectReportServiceTests()
    {
        _service = new HtmlProjectReportService();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void GenerateProjectReport_ReturnsReport_WhenTasksExist()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var expectedTitle = "Test Project";

        var tasks = _fixture.Build<TaskManager.Core.Models.Task>()
            .With(task => task.Project, new Project { Title = expectedTitle })
            .CreateMany(5)
            .ToList();

        // Act
        var report = _service.GenerateProjectReport(projectId, tasks);

        // Assert
        Assert.Contains($"<h1>Отчет по проекту: {expectedTitle}</h1>", report);
        Assert.Contains("<table border='1'>", report);
        Assert.Contains(tasks.First().Title, report);
    }

    [Fact]
    public void GenerateProjectReport_ReturnsNoTasksMessage_WhenNoTasksExist()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var tasks = new List<TaskManager.Core.Models.Task>();

        // Act
        var report = _service.GenerateProjectReport(projectId, tasks);

        // Assert
        Assert.Contains($"<h1>Отчет по проекту {projectId}</h1>", report);
        Assert.Contains("Нет задач для данного проекта.", report);
    }
}
