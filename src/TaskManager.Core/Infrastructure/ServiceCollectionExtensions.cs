using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces.Services.Authentication;
using TaskManager.Core.Interfaces.Services.Core;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Interfaces.Services.Scheduling;
using TaskManager.Core.Models;
using TaskManager.Core.Services.Authentication;
using TaskManager.Core.Services.Base;
using TaskManager.Core.Services.ProjectManagement;
using TaskManager.Core.Services.Scheduling;

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

        services.AddScoped<INewsletterReportService, NewsletterReportService>();
        services.AddScoped<IDailyNewsletterSchedulerService, DailyNewsletterSchedulerService>();
        

        return services;
    }
}
