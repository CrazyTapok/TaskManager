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

        return services;
    }
}
