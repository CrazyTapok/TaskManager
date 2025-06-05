using AutoFixture;
using Moq;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class NewsletterReportServiceTests
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly Mock<IProjectReportService> _mockReportService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly NewsletterReportService _newsletterReportService;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public NewsletterReportServiceTests()
    {
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockProjectService = new Mock<IProjectService>();
        _mockTaskService = new Mock<ITaskService>();
        _mockReportService = new Mock<IProjectReportService>();
        _mockEmailService = new Mock<IEmailService>();
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        _newsletterReportService = new NewsletterReportService(
            _mockEmployeeService.Object,
            _mockProjectService.Object,
            _mockTaskService.Object,
            _mockReportService.Object,
            _mockEmailService.Object);

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task ExecuteDailyJob_ShouldSendEmailsToSubscribedEmployees()
    {
        // Arrange
        var employees = _fixture.Build<Employee>()
            .With(employee => employee.IsDailyNewsletterEnabled, true)
            .CreateMany(2).ToList();

        var projects = _fixture.Build<Project>().CreateMany(2).ToList();
        var reportContent = "Test Report Content";

        _mockEmployeeService.Setup(employeeService => employeeService.GetEmployeesWithDailyNewsletterEnabledAsync(_cancellationToken))
            .ReturnsAsync(employees);

        _mockProjectService.Setup(projectService => projectService.GetProjectsByEmployeeIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(projects);

        _mockReportService.Setup(reportService => reportService.GenerateProjectReportAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(reportContent);

        _mockEmailService.Setup(emailService => emailService.SendEmailAsync(It.IsAny<EmailNotification>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _newsletterReportService.ExecuteDailyJob(_cancellationToken);

        // Assert
        _mockEmployeeService.Verify(employeeService => employeeService.GetEmployeesWithDailyNewsletterEnabledAsync(_cancellationToken), Times.Once);
        _mockProjectService.Verify(projectService => projectService.GetProjectsByEmployeeIdAsync(It.IsAny<Guid>(), _cancellationToken), Times.Exactly(employees.Count));
        _mockReportService.Verify(reportService => reportService.GenerateProjectReportAsync(It.IsAny<Guid>(), _cancellationToken), Times.Exactly(employees.Count * projects.Count));
        _mockEmailService.Verify(emailService => emailService.SendEmailAsync(It.IsAny<EmailNotification>(), _cancellationToken), Times.Exactly(employees.Count * projects.Count));
    }
}
