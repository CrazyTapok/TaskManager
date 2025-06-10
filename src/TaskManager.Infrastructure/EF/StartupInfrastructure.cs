using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Services.Email;
using TaskManager.Infrastructure.Services.Reporting;
using TaskManager.Core.Interfaces.Services.Email;
using TaskManager.Core.Interfaces.Services.Scheduling;
using TaskManager.Core.Interfaces.Services.ProjectManagement;

namespace TaskManager.Infrastructure.EF;

public static class StartupInfrastructure
{
    public static void AddDbContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<Core.Models.Task>, TaskRepository>();

        services.AddScoped<IJobScheduler, HangfireJobScheduler>();
        services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IProjectReportService, HtmlProjectReportService>();


        services.AddHealthChecks()
                   .AddDbContextCheck<DBContext>();
    }
}