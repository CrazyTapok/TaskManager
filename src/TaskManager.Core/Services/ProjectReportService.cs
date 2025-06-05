using System.Text;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Core.Services;

internal class ProjectReportService : IProjectReportService
{
    private readonly ITaskService _taskService;

    public ProjectReportService(ITaskService taskService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    public async Task<string> GenerateProjectReportAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetTasksByProjectIdAsync(projectId, cancellationToken);

        if (!tasks.Any())
        {
            return $"<html><body><h1>Отчет по проекту {projectId}</h1><p>Нет задач для данного проекта.</p></body></html>";
        }

        var projectName = tasks.First().Project.Title;
        var report = new StringBuilder();

        report.Append($"<html><body><h1>Отчет по проекту: {projectName}</h1>");
        report.Append("<table border='1'><tr><th>Название</th><th>Описание</th><th>Статус</th><th>Ответственный</th><th>Дата создания</th><th>Дата обновления</th></tr>");

        foreach (var task in tasks)
        {
            report.Append($"<tr><td>{task.Title}</td><td>{task.Description}</td><td>{task.Status}</td><td>{task.AssignedEmployee?.Name ?? "Не назначено"}</td><td>{task.CreatedDate:dd-MM-yyyy}</td><td>{task.UpdatedDate:dd-MM-yyyy}</td></tr>");
        }

        report.Append("</table></body></html>");
        return report.ToString();
    }
}

