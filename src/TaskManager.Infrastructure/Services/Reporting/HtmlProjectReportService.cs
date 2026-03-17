using System.Text;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Infrastructure.Services.Reporting;

internal class HtmlProjectReportService : IProjectReportService
{
    public string GenerateProjectReport(Guid projectId, List<Task> tasks)
    {
        if (tasks.Count == 0)
        {
            return $"<html><body><h1>Отчет по проекту {projectId}</h1><p>Нет задач для данного проекта.</p></body></html>";
        }

        var projectName = tasks.First().Project.Title;
        var report = new StringBuilder();

        report.AppendLine($"<html><body><h1>Отчет по проекту: {projectName}</h1>");
        report.AppendLine("<table border='1'><tr><th>Название</th><th>Описание</th><th>Статус</th><th>Ответственный</th><th>Дата создания</th><th>Дата обновления</th></tr>");

        foreach (var task in tasks)
        {
            report.AppendLine($"<tr><td>{task.Title}</td><td>{task.Description}</td><td>{task.Status}</td><td>{task.AssignedEmployee?.Name ?? "Не назначено"}</td><td>{task.CreatedDate:dd-MM-yyyy}</td><td>{task.UpdatedDate:dd-MM-yyyy}</td></tr>");
        }

        report.AppendLine("</table></body></html>");


        return report.ToString();
    }
}

