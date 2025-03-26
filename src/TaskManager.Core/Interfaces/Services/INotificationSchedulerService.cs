using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface INotificationSchedulerService
{
    void ScheduleEmailJob(EmailNotification emailNotification);
}
