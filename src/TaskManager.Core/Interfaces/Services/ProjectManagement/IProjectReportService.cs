using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Services.ProjectManagement;


public interface IProjectReportService
{
    string GenerateProjectReport(Guid projectId, List<Task> tasks);
}
