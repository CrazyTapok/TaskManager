using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Tests.API.Controllers;

public class NotificationControllerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<INotificationSchedulerService> _notificationSchedulerServiceMock;
    private readonly NotificationController _notificationController;

    public NotificationControllerTests()
    {
        _fixture = new Fixture();
        _notificationSchedulerServiceMock = new Mock<INotificationSchedulerService>();
        _notificationController = new NotificationController(_notificationSchedulerServiceMock.Object);
    }

    [Fact]
    public void ScheduleEmailJob_ReturnsOkResult_WhenJobIsScheduled()
    {
        // Arrange
        var emailNotificationRequest = _fixture.Build<EmailNotificationRequest>()
            .With(notification => notification.CronExpression, "0/5 * * * *")
            .With(notification => notification.EmailList, new List<string> { "test@example.com" })
            .With(notification => notification.CreatedByName, _fixture.Create<string>())
            .With(notification => notification.CreatedByEmail, "testuser@example.com")
            .With(notification => notification.Subject, _fixture.Create<string>())
            .With(notification => notification.Body, _fixture.Create<string>())
            .Create();

        // Act
        var result = _notificationController.ScheduleEmailJob(emailNotificationRequest);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        _notificationSchedulerServiceMock.Verify(service =>
            service.ScheduleEmailJob(It.IsAny<EmailNotification>()), Times.Once);
    }
}
