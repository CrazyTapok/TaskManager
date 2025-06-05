using AutoFixture;
using MimeKit;
using Moq;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Models;
using Microsoft.Extensions.Options;
using TaskManager.Core.Services;
using TaskManager.Core.Interfaces.Services;
using Task = System.Threading.Tasks.Task;
using MailKit.Security;

namespace TaskManager.Tests.Core.Services;

public class EmailServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettingsMock;
    private readonly Mock<ISmtpClientWrapper> _smtpClientMock;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _fixture = new Fixture();
        _smtpSettingsMock = new Mock<IOptions<SmtpSettings>>();
        _smtpClientMock = new Mock<ISmtpClientWrapper>();

        var smtpSettings = _fixture.Build<SmtpSettings>()
            .With(smtp => smtp.Host, "smtp.test.com")
            .With(smtp => smtp.Port, 587)
            .With(smtp => smtp.User, "test@test.com")
            .With(smtp => smtp.Password, "password")
            .Create();

        _smtpSettingsMock.Setup(s => s.Value).Returns(smtpSettings);

        // Мокируем сетевые вызовы и проверяем, что они вызываются
        _smtpClientMock.Setup(client => client.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock.Setup(client => client.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock.Setup(client => client.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _emailService = new EmailService(_smtpSettingsMock.Object, _smtpClientMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldSendEmailSuccessfully()
    {
        // Arrange
        var emailNotification = _fixture.Create<EmailNotification>();

        // Act
        await _emailService.SendEmailAsync(emailNotification);

        // Assert
        _smtpClientMock.Verify(client => client.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()), Times.Once);

        _smtpClientMock.Verify(client => client.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _smtpClientMock.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        _smtpClientMock.Verify(client => client.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
