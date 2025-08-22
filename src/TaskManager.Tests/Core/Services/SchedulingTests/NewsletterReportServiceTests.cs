using AutoFixture;
using Moq;
using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Models;
using TaskManager.Core.Services.Scheduling;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services.SchedulingTests;

public class NewsletterReportServiceTests
{
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IProjectReportService> _mockReportService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly NewsletterReportService _newsletterReportService;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public NewsletterReportServiceTests()
    {
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockReportService = new Mock<IProjectReportService>();
        _mockEmailService = new Mock<IEmailService>();
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        _newsletterReportService = new NewsletterReportService(
            _mockEmployeeService.Object,
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
            .With(employee => employee.Projects, _fixture.CreateMany<Project>().ToList())
            .CreateMany(2).ToList();

        var reportContent = "Test Report Content";

        _mockEmployeeService.Setup(service => service.GetEmployeesWithDailyNewsletterEnabledAsync(_cancellationToken))
            .ReturnsAsync(employees);

        _mockReportService.Setup(service => service.GenerateProjectReport(It.IsAny<Guid>(), It.IsAny<List<TaskManager.Core.Models.Task>>()))
            .Returns(reportContent);

        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<EmailNotification>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _newsletterReportService.SendDailyProjectReportsAsync(_cancellationToken);

        // Assert
        _mockEmployeeService.Verify(service => service.GetEmployeesWithDailyNewsletterEnabledAsync(_cancellationToken), Times.Once);
        _mockReportService.Verify(service => service.GenerateProjectReport(It.IsAny<Guid>(), It.IsAny<List<TaskManager.Core.Models.Task>>()), Times.Exactly(employees.Sum(e => e.Projects.Count)));
        _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<EmailNotification>(), _cancellationToken), Times.Exactly(employees.Sum(e => e.Projects.Count)));
    }
}
