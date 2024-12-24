using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Core.Infrastructure;

public static class ServiceCollection
{
    public static IServiceCollection AddServiceModule(this IServiceCollection services)
    {
        services.AddTransient<IEmployeeService, EmployeeService>(); 
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient(typeof(IService<Company>), typeof(Service<Company>)); 

        return services;
    }
}
