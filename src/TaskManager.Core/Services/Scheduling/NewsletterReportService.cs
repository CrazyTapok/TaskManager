using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Interfaces.Services.Scheduling;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services.Scheduling;

internal class NewsletterReportService : INewsletterReportService
{
    private readonly IEmployeeService _employeeService;
    private readonly IProjectReportService _reportService;
    private readonly IEmailService _emailService;

    public NewsletterReportService(
        IEmployeeService employeeService,
        IProjectReportService reportService,
        IEmailService emailService)
    {
        _employeeService = employeeService;
        _reportService = reportService;
        _emailService = emailService;
    }

    public async Task SendDailyProjectReportsAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesWithDailyNewsletterEnabledAsync(cancellationToken);

        var tasks = new List<Task>();

        foreach (var employee in employees)
        {
            foreach (var project in employee.Projects)
            {
                var reportContent = _reportService.GenerateProjectReport(project.Id, project.Tasks);

                var emailNotification = new EmailNotification
                {
                    Subject = $"Ежедневный отчет по проекту: {project.Title}",
                    RecipientName = employee.Name,
                    RecipientEmail = employee.Email,
                    Body = reportContent
                };

                tasks.Add(_emailService.SendEmailAsync(emailNotification, cancellationToken));
            }
        }

        await Task.WhenAll(tasks);
    }
}
