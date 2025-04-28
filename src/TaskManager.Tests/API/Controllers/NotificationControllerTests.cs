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
    private readonly Mock<IProjectNotificationService> _projectNotificationServiceMock;
    private readonly NotificationController _controller;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public NotificationControllerTests()
    {
        _projectNotificationServiceMock = new Mock<IProjectNotificationService>();
        _controller = new NotificationController(_projectNotificationServiceMock.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void EnableNotifications_ReturnsOk_WhenCalled()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();
        var request = _fixture.Create<EmailNotificationRequest>();

        _projectNotificationServiceMock
            .Setup(projectNotificationService => projectNotificationService.EnableNotifications(projectId, It.IsAny<EmailNotification>(), _cancellationToken));

        // Act
        var result = _controller.EnableNotifications(projectId, request);

        // Assert
        _projectNotificationServiceMock.Verify(
            projectNotificationService => projectNotificationService.EnableNotifications(projectId, It.IsAny<EmailNotification>(), _cancellationToken), Times.Once);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void DisableNotifications_ReturnsOk_WhenCalled()
    {
        // Arrange
        var projectId = _fixture.Create<Guid>();

        _projectNotificationServiceMock
            .Setup(projectNotificationService => projectNotificationService.DisableNotifications(projectId, _cancellationToken));

        // Act
        var result = _controller.DisableNotifications(projectId);

        // Assert
        _projectNotificationServiceMock.Verify(projectNotificationService => projectNotificationService.DisableNotifications(projectId, _cancellationToken), Times.Once);
        Assert.IsType<OkResult>(result);
    }
}
