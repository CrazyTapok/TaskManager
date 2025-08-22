namespace TaskManager.Core.Interfaces.Services.Scheduling;

public interface INewsletterReportService
{
    Task SendDailyProjectReportsAsync(CancellationToken cancellationToken = default);
}
