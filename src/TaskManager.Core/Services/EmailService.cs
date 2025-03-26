using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Core.Services;

internal class EmailService : IEmailService
{
    private readonly string _smtpUser;
    private readonly string _smtpPassword;

    public EmailService()
    {
        _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new ArgumentNullException(nameof(_smtpUser));
        _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new ArgumentNullException(nameof(_smtpPassword));
    }

    public async Task SendEmailAsync(EmailNotification emailNotification)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPassword);

        foreach (var recipientEmail in emailNotification.EmailList)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailNotification.CreatedByName, emailNotification.CreatedByEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = emailNotification.Subject;

            var builder = new BodyBuilder { HtmlBody = emailNotification.Body };
            message.Body = builder.ToMessageBody();

            await client.SendAsync(message);
        }

        await client.DisconnectAsync(true);
    }
}