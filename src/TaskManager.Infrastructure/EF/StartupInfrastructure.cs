using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.EF;

public static class StartupInfrastructure
{
    public static void AddDbContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); 
        
        services.AddScoped<IRepository<Employee>, EmployeeRepository>(); 
        services.AddScoped<IRepository<Project>, ProjectRepository>(); 
        services.AddScoped<IRepository<Core.Models.Task>, TaskRepository>();

        services.AddHealthChecks()
                   .AddDbContextCheck<DBContext>();
    }
}
