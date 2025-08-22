using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Email;
using TaskManager.Infrastructure.Services.Email;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Infrastructure.Services.Email;

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

        _smtpSettingsMock.Setup(smtp => smtp.Value).Returns(smtpSettings);

        _smtpClientMock
            .Setup(client => client.ConnectAsync(It.IsAny<SmtpConnectionOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock
            .Setup(client => client.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock
            .Setup(client => client.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        _smtpClientMock
            .Setup(client => client.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
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
        _smtpClientMock.Verify(client => client.ConnectAsync(
                It.Is<SmtpConnectionOptions>(option =>
                    option.Host == "smtp.test.com" &&
                    option.Port == 587 &&
                    option.UseSsl == true
                ), 
                It.IsAny<CancellationToken>()),
                Times.Once);

        _smtpClientMock.Verify(client => client.AuthenticateAsync("test@test.com", "password", It.IsAny<CancellationToken>()), Times.Once);

        _smtpClientMock.Verify(client =>
            client.SendAsync(It.Is<EmailMessage>(message =>
                message.Subject == emailNotification.Subject &&
                message.Body == emailNotification.Body &&
                message.To == emailNotification.RecipientEmail
            ), 
            It.IsAny<CancellationToken>()),
            Times.Once);

        _smtpClientMock.Verify(client => client.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
