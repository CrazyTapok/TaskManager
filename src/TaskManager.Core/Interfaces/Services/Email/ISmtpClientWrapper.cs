using TaskManager.Core.Models.Email;

namespace TaskManager.Core.Interfaces.Services.Email;

public interface ISmtpClientWrapper : IDisposable
{
    Task ConnectAsync(SmtpConnectionOptions options, CancellationToken cancellationToken = default);
    Task AuthenticateAsync(string user, string password, CancellationToken cancellationToken = default);
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default);
}