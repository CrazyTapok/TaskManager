using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Core.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceModule(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>(); 
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IService<Company>, Service<Company>>();
        
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddSingleton<IJobSchedulerWrapper, JobSchedulerWrapper>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IProjectReportService, ProjectReportService>();
        services.AddScoped<INewsletterReportService, NewsletterReportService>();
        services.AddScoped<IDailyNewsletterSchedulerService, DailyNewsletterSchedulerService>();
        

        return services;
    }
}
