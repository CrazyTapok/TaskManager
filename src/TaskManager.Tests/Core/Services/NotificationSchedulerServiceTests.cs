using Hangfire.MemoryStorage;
using AutoFixture;
using Moq;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Hangfire;

namespace TaskManager.Tests.Core.Services;

public class NotificationSchedulerServiceTests
{
    private readonly Fixture _fixture;

    public NotificationSchedulerServiceTests()
    {
        _fixture = new Fixture();
        JobStorage.Current = new MemoryStorage();
    }

    [Fact]
    public void ScheduleEmailJob_ValidatesParameters()
    {
        var cronExpression = "0/5 * * * *";

        // Arrange
        var emailServiceMock = new Mock<IEmailService>();
        var notificationSchedulerService = new NotificationSchedulerService(emailServiceMock.Object);

        var emailNotification = _fixture.Build<EmailNotification>()
            .With(notification => notification.JobId, _fixture.Create<string>())
            .With(notification => notification.CronExpression, cronExpression)
            .Create();

        // Act
        notificationSchedulerService.ScheduleEmailJob(emailNotification);

        // Assert
        Assert.NotNull(emailNotification.JobId);
        Assert.Equal(cronExpression, emailNotification.CronExpression);
        emailServiceMock.Verify(service => service.SendEmailAsync(emailNotification), Times.Never);
    }
}