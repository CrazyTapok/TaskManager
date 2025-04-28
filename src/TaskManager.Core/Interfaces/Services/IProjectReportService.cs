namespace TaskManager.Core.Interfaces.Services;

public interface IProjectReportService
{
    Task<string> GenerateProjectReportAsync(Guid projectId, CancellationToken cancellationToken = default);
}
