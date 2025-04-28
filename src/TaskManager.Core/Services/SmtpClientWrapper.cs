using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ISmtpClient = TaskManager.Core.Interfaces.Services.ISmtpClient;

namespace TaskManager.Core.Services;

internal class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient = new();

    public async Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken = default)
    {
        await _smtpClient.ConnectAsync(host, port, options, cancellationToken);
    }

    public async Task AuthenticateAsync(string user, string password, CancellationToken cancellationToken = default)
    {
        await _smtpClient.AuthenticateAsync(user, password, cancellationToken);
    }

    public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        await _smtpClient.SendAsync(message, cancellationToken);
    }

    public async Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
    {
        await _smtpClient.DisconnectAsync(quit, cancellationToken);
    }

    public void Dispose() => _smtpClient.Dispose();
}