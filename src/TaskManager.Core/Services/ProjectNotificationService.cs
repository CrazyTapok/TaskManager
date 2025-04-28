using Hangfire;
using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Utilities;

namespace TaskManager.Core.Services;

internal class ProjectNotificationService(
    IRecurringJobManager recurringJobManager,
    IProjectReportService projectReportService,
    IEmailService emailService,
    IOptions<NotificationSettings> notificationSettings) : IProjectNotificationService
{
    private readonly NotificationSettings _settings = notificationSettings?.Value ?? throw new ArgumentNullException(nameof(notificationSettings));

    public void EnableNotifications(Guid projectId, EmailNotification emailNotification, CancellationToken cancellationToken = default)
    {
        var jobKey = JobKeyGenerator.GenerateProjectNotificationKey(projectId);

        recurringJobManager.AddOrUpdate(
            jobKey,
            () => SendProjectReport(projectId, emailNotification, cancellationToken),
            Cron.Daily(_settings.Hour, _settings.Minute)
        );
    }

    public void DisableNotifications(Guid projectId, CancellationToken cancellationToken = default)
    {
        var jobKey = JobKeyGenerator.GenerateProjectNotificationKey(projectId);
        recurringJobManager.RemoveIfExists(jobKey);
    }

    public void SendProjectReport(Guid projectId, EmailNotification emailNotification, CancellationToken cancellationToken = default)
    {
        var report = projectReportService.GenerateProjectReportAsync(projectId, cancellationToken);
        emailService.SendEmailAsync(projectId, emailNotification, cancellationToken);
    }
}
