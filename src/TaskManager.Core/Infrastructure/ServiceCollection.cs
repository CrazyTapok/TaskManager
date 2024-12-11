using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Services;

namespace TaskManager.Core.Infrastructure;

public static class ServiceCollection
{
    public static IServiceCollection AddServiceModule(this IServiceCollection services)
    {
        services.AddTransient<IEmployeeService, EmployeeService>(); 
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient <ITaskService, TaskService>();

        return services;
    }
}
