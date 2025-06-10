using MailKit.Security;
using MimeKit;

namespace TaskManager.Core.Interfaces.Services.Email;

public interface ISmtpClientWrapper : IDisposable
{
    Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken = default);
    Task AuthenticateAsync(string user, string password, CancellationToken cancellationToken = default);
    Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default);
}