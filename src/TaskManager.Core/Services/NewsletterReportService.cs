using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

internal class NewsletterReportService : INewsletterReportService
{
    private readonly IEmployeeService _employeeService;
    private readonly IProjectService _projectService;
    private readonly IProjectReportService _reportService;
    private readonly IEmailService _emailService;

    public NewsletterReportService(
        IEmployeeService employeeService,
        IProjectService projectService,
        ITaskService taskService,
        IProjectReportService reportService,
        IEmailService emailService)
    {
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task ExecuteDailyJob(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesWithDailyNewsletterEnabledAsync(cancellationToken);

        foreach (var employee in employees)
        {
            var projects = await _projectService.GetProjectsByEmployeeIdAsync(employee.Id, cancellationToken);
            foreach (var project in projects)
            {
                var reportContent = await _reportService.GenerateProjectReportAsync(project.Id, cancellationToken);

                var emailNotification = new EmailNotification
                {
                    Subject = $"Ежедневный отчет по проекту: {project.Title}",
                    RecipientName = employee.Name,
                    RecipientEmail = employee.Email,
                    Body = reportContent
                };

                await _emailService.SendEmailAsync(emailNotification, cancellationToken);
            }
        }
    }
}
