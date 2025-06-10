using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Interfaces.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(EmailNotification emailNotification, CancellationToken cancellationToken = default);
}

