namespace TaskManager.Core.Interfaces.Services.Scheduling;

public interface INewsletterReportService
{
    Task ExecuteDailyJob(CancellationToken cancellationToken = default);
}
