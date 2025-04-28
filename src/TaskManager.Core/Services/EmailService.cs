using MimeKit;
using MailKit.Security;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;
using TaskManager.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using ISmtpClient = TaskManager.Core.Interfaces.Services.ISmtpClient;

namespace TaskManager.Core.Services;

internal class EmailService(IOptions<SmtpSettings> smtpSettings, ISmtpClient smtpClient) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings?.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
    private readonly ISmtpClient _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));

    public async Task SendEmailAsync(Guid id, EmailNotification emailNotification, CancellationToken cancellationToken = default)
    {
        await _smtpClient.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
        await _smtpClient.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password, cancellationToken);

        var message = new MimeMessage
        {
            Subject = emailNotification.Subject
        };

        message.From.Add(new MailboxAddress("Task Manager App", _smtpSettings.User));
        message.To.Add(new MailboxAddress(emailNotification.RecipientName, emailNotification.RecipientEmail));

        var builder = new BodyBuilder { HtmlBody = emailNotification.Body };
        message.Body = builder.ToMessageBody();

        await _smtpClient.SendAsync(message, cancellationToken);
        await _smtpClient.DisconnectAsync(true, cancellationToken);
    }
}