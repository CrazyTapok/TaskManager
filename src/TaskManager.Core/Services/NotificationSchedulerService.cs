using Hangfire;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

internal class NotificationSchedulerService(IEmailService emailService) : INotificationSchedulerService
{
    private readonly IEmailService _emailService = emailService;

    public void ScheduleEmailJob(EmailNotification emailNotification)
    {
        RecurringJob.AddOrUpdate(
            emailNotification.JobId,
            () => _emailService.SendEmailAsync(emailNotification),
            emailNotification.CronExpression);
    }
}