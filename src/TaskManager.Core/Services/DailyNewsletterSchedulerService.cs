using Hangfire;
using Microsoft.Extensions.Options;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Infrastructure.Configuration;

internal class DailyNewsletterSchedulerService : IDailyNewsletterSchedulerService
{
    private readonly INewsletterReportService _newsletterReportService;
    private readonly NotificationSettings _notificationSettings;
    private readonly IJobSchedulerWrapper _jobSchedulerWrapper;

    public DailyNewsletterSchedulerService(
        INewsletterReportService newsletterJobService,
        IOptions<NotificationSettings> notificationSettings,
        IJobSchedulerWrapper jobSchedulerWrapper)
    {
        _newsletterReportService = newsletterJobService ?? throw new ArgumentNullException(nameof(newsletterJobService));
        _notificationSettings = notificationSettings?.Value ?? throw new ArgumentNullException(nameof(notificationSettings));
        _jobSchedulerWrapper = jobSchedulerWrapper ?? throw new ArgumentNullException(nameof(jobSchedulerWrapper));
    }

    public void ConfigureJobs()
    {
        var hours = _notificationSettings.NotificationTime.Hours;
        var minutes = _notificationSettings.NotificationTime.Minutes;

        _jobSchedulerWrapper.ScheduleJob(
            jobId: "daily-newsletter-job",
            () => _newsletterReportService.ExecuteDailyJob(default),
            cronExpression: Cron.Daily(hours, minutes));
    }
}

