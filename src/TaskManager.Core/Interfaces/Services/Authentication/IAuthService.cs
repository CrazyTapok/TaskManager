namespace TaskManager.Core.Interfaces.Services.Authentication;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}