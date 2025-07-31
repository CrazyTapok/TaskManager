using Microsoft.Extensions.Options;
using TaskManager.Core.Infrastructure.Configuration;
using TaskManager.Core.Interfaces.Services.Scheduling;

namespace TaskManager.Core.Services.Scheduling;

internal class DailyNewsletterSchedulerService : IDailyNewsletterSchedulerService
{
    private readonly INewsletterReportService _newsletterReportService;
    private readonly NotificationSettings _notificationSettings;
    private readonly IJobScheduler _jobSchedulerWrapper;

    public DailyNewsletterSchedulerService(
        INewsletterReportService newsletterJobService,
        IOptions<NotificationSettings> notificationSettings,
        IJobScheduler jobSchedulerWrapper)
    {
        _newsletterReportService = newsletterJobService;
        _jobSchedulerWrapper = jobSchedulerWrapper;
        _notificationSettings = notificationSettings?.Value ?? throw new ArgumentNullException(nameof(notificationSettings));
    }

    public void ConfigureJobs()
    {
        _jobSchedulerWrapper.ScheduleJob(
            jobId: _notificationSettings.Id,
            () => _newsletterReportService.ExecuteDailyJob(default),
            cronExpression: _notificationSettings.CronExpression);
    }
}

