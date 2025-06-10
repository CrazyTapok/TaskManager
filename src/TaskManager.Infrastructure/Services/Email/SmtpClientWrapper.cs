using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TaskManager.Core.Interfaces.Services.Email;

namespace TaskManager.Infrastructure.Services.Email;

internal class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient = new();

    public Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken = default)
    {
        return _smtpClient.ConnectAsync(host, port, options, cancellationToken);
    }

    public Task AuthenticateAsync(string user, string password, CancellationToken cancellationToken = default)
    {
        return _smtpClient.AuthenticateAsync(user, password, cancellationToken);
    }

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        return _smtpClient.SendAsync(message, cancellationToken);
    }

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
    {
        return _smtpClient.DisconnectAsync(quit, cancellationToken);
    }

    public void Dispose() => _smtpClient.Dispose();
}