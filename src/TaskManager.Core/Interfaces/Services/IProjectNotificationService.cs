using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Interfaces.Services;

public interface IProjectNotificationService
{
    void EnableNotifications(Guid projectId, EmailNotification emailNotification, CancellationToken cancellationToken = default);

    void DisableNotifications(Guid projectId, CancellationToken cancellationToken = default);

    void SendProjectReport(Guid projectId, EmailNotification emailNotification, CancellationToken cancellationToken = default);
}
