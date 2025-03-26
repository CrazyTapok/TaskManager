using AutoFixture;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class EmailServiceTests
{
    private readonly EmailService _emailService;
    private readonly Fixture _fixture;

    public EmailServiceTests()
    {
        Environment.SetEnvironmentVariable("SMTP_USER", "test@gmail.com");
        Environment.SetEnvironmentVariable("SMTP_PASSWORD", "testPasword");

        _emailService = new EmailService();

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task SendEmailAsync_SendsEmailSuccessfully()
    {
        // Arrange
        var emailNotification = _fixture.Build<EmailNotification>()
            .With(notification => notification.EmailList, new List<string> { "recipient1@example.com", "recipient2@example.com" })
            .With(notification => notification.CreatedByName, _fixture.Create<string>())
            .With(notification => notification.CreatedByEmail, "sender@example.com")
            .Create();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _emailService.SendEmailAsync(emailNotification));
        Assert.Null(exception);
    }
}