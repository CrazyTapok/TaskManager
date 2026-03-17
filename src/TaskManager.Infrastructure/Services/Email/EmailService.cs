using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Email;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Services.Email;

internal class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ISmtpClientWrapper _smtpClient;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ISmtpClientWrapper smtpClient)
    {
        _smtpSettings = smtpSettings?.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(EmailNotification emailNotification, CancellationToken cancellationToken = default)
    {
        var connectionOptions = new SmtpConnectionOptions
        {
            Host = _smtpSettings.Host,
            Port = _smtpSettings.Port,
            UseSsl = true
        };

        await _smtpClient.ConnectAsync(connectionOptions, cancellationToken);
        await _smtpClient.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password, cancellationToken);

        var message = new EmailMessage
        {
            From = _smtpSettings.User,
            To = emailNotification.RecipientEmail,
            Subject = emailNotification.Subject,
            Body = emailNotification.Body
        };

        await _smtpClient.SendAsync(message, cancellationToken);
        await _smtpClient.DisconnectAsync(quit: true, cancellationToken);
    }
}