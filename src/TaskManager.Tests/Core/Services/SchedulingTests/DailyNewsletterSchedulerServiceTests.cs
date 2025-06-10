using Hangfire;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services.Scheduling;
using TaskManager.Core.Services.Scheduling;

namespace TaskManager.Tests.Core.Services.SchedulingTests;

public class DailyNewsletterSchedulerServiceTests
{
    private const string DailyNewsletterJobName = "daily-newsletter-job";
    private static readonly TimeSpan NotificationTime = new TimeSpan(9, 30, 0);

    private readonly Mock<INewsletterReportService> _mockNewsletterReportService;
    private readonly Mock<IJobScheduler> _mockJobSchedulerWrapper;
    private readonly Mock<IOptions<NotificationSettings>> _mockNotificationSettings;
    private readonly DailyNewsletterSchedulerService _dailyNewsletterSchedulerService;

    public DailyNewsletterSchedulerServiceTests()
    {
        _mockNewsletterReportService = new Mock<INewsletterReportService>();
        _mockJobSchedulerWrapper = new Mock<IJobScheduler>();
        _mockNotificationSettings = new Mock<IOptions<NotificationSettings>>();

        var notificationSettings = new NotificationSettings
        {
            Id = DailyNewsletterJobName,
            Time = NotificationTime
        };

        _mockNotificationSettings.Setup(setting => setting.Value).Returns(notificationSettings);

        _dailyNewsletterSchedulerService = new DailyNewsletterSchedulerService(
            _mockNewsletterReportService.Object,
            _mockNotificationSettings.Object,
            _mockJobSchedulerWrapper.Object);
    }

    [Fact]
    public void ConfigureJobs_ShouldRegisterRecurringJob()
    {
        // Arrange
        _mockJobSchedulerWrapper.Setup(wrapper => wrapper.ScheduleJob(
                DailyNewsletterJobName,
                It.IsAny<Expression<Action>>(),
                Cron.Daily(NotificationTime.Hours, NotificationTime.Minutes)))
            .Verifiable();

        // Act
        _dailyNewsletterSchedulerService.ConfigureJobs();

        // Assert
        _mockJobSchedulerWrapper.Verify(wrapper => wrapper.ScheduleJob(
            DailyNewsletterJobName,
            It.IsAny<Expression<Action>>(),
            Cron.Daily(NotificationTime.Hours, NotificationTime.Minutes)), Times.Once);
    }
}