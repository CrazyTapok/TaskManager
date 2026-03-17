using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Models.Email;

namespace TaskManager.Infrastructure.Services.Email;

internal class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient = new();

    public Task ConnectAsync(SmtpConnectionOptions options, CancellationToken cancellationToken = default)
    {
        var secureOption = options.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
       
        return _smtpClient.ConnectAsync(options.Host, options.Port, secureOption, cancellationToken);
    }

    public Task AuthenticateAsync(string user, string password, CancellationToken cancellationToken = default)
    {
        return _smtpClient.AuthenticateAsync(user, password, cancellationToken);
    }

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(message.From));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart("html") { Text = message.Body };

        return _smtpClient.SendAsync(mimeMessage, cancellationToken);
    }

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
    {
        return _smtpClient.DisconnectAsync(quit, cancellationToken);
    }

    public void Dispose() => _smtpClient.Dispose();
}