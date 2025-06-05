namespace TaskManager.Core.Interfaces.Services;

public interface INewsletterReportService
{
    Task ExecuteDailyJob(CancellationToken cancellationToken = default);
}
