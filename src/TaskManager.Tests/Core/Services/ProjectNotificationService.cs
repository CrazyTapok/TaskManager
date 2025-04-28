using AutoFixture;
using Hangfire;
using Hangfire.Common;
using Moq;
using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Core.Utilities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class ProjectNotificationServiceTests
{
    private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
    private readonly Mock<IProjectReportService> _projectReportServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IOptions<NotificationSettings>> _notificationSettingsMock;
    private readonly ProjectNotificationService _notificationService;
    private readonly CancellationToken _cancellationToken;
    private readonly Fixture _fixture;

    public ProjectNotificationServiceTests()
    {
        _recurringJobManagerMock = new Mock<IRecurringJobManager>();
        _projectReportServiceMock = new Mock<IProjectReportService>();
        _emailServiceMock = new Mock<IEmailService>();
        _notificationSettingsMock = new Mock<IOptions<NotificationSettings>>();

        var notificationSettings = new NotificationSettings { Hour = 8, Minute = 0 };
        _notificationSettingsMock.Setup(notification => notification.Value).Returns(notificationSettings);

        _notificationService = new ProjectNotificationService(
            _recurringJobManagerMock.Object,
            _projectReportServiceMock.Object,
            _emailServiceMock.Object,
            _notificationSettingsMock.Object);

        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void EnableNotifications_SchedulesRecurringJob()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var emailNotification = _fixture.Create<EmailNotification>();
        var jobKey = JobKeyGenerator.GenerateProjectNotificationKey(projectId);

        _recurringJobManagerMock.Setup(recurringMock => recurringMock.AddOrUpdate(
            jobKey,
            It.Is<Job>(job => job.Method.Name.Contains("SendProjectReport")),
            Cron.Daily(8, 0),
            It.IsAny<RecurringJobOptions>()));

        // Act
        _notificationService.EnableNotifications(projectId, emailNotification);

        // Assert
        _recurringJobManagerMock.Verify(recurringMock => recurringMock.AddOrUpdate(
            jobKey,
            It.Is<Job>(job => job.Method.Name.Contains("SendProjectReport")),
            Cron.Daily(8, 0),
            It.IsAny<RecurringJobOptions>()), Times.Once);
    }

    [Fact]
    public void DisableNotifications_RemovesRecurringJob()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var jobKey = JobKeyGenerator.GenerateProjectNotificationKey(projectId);

        _recurringJobManagerMock.Setup(recurringMock => recurringMock.RemoveIfExists(jobKey));

        // Act
        _notificationService.DisableNotifications(projectId);

        // Assert
        _recurringJobManagerMock.Verify(recurringMock => recurringMock.RemoveIfExists(jobKey), Times.Once);
    }

    [Fact]
    public void SendProjectReportAsync_GeneratesReportAndSendsEmail()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var emailNotification = _fixture.Create<EmailNotification>();
        var report = _fixture.Create<string>();

        _projectReportServiceMock.Setup(projectReport => projectReport.GenerateProjectReportAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        _emailServiceMock.Setup(emailService => emailService.SendEmailAsync(projectId, emailNotification, _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _notificationService.SendProjectReport(projectId, emailNotification);

        // Assert
        _projectReportServiceMock.Verify(
            projectReport => projectReport.GenerateProjectReportAsync(projectId, It.IsAny<CancellationToken>()),
            Times.Once);

        _emailServiceMock.Verify(
            emailService => emailService.SendEmailAsync(projectId, emailNotification, _cancellationToken),
            Times.Once);
    }
}
