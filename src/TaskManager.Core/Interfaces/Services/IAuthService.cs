namespace TaskManager.Core.Interfaces.Services;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}