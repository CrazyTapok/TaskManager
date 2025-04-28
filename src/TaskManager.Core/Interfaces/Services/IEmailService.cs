using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(Guid Id, EmailNotification emailNotification, CancellationToken cancellationToken = default);
}

