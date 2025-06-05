using MimeKit;
using MailKit.Security;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;
using TaskManager.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using ISmtpClientWrapper = TaskManager.Core.Interfaces.Services.ISmtpClientWrapper;

namespace TaskManager.Core.Services;

internal class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ISmtpClientWrapper _smtpClient;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ISmtpClientWrapper smtpClient)
    {
        _smtpSettings = smtpSettings?.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
        _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));
    }

    public async Task SendEmailAsync(EmailNotification emailNotification, CancellationToken cancellationToken = default)
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
        await _smtpClient.DisconnectAsync(quit: true, cancellationToken);
    }
}