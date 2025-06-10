namespace TaskManager.Core.Interfaces.Services.ProjectManagement;
using Task = Models.Task;

public interface IProjectReportService
{
    string GenerateProjectReport(Guid projectId, List<Task> tasks);
}
