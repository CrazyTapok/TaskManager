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
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<Core.Models.Task>, TaskRepository>();

        services.AddHealthChecks()
                   .AddDbContextCheck<DBContext>();
    }
}