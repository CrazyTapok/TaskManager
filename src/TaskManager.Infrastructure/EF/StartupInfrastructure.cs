using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infrastructure.EF;
public static class StartupInfrastructure
{
    public static void AddDbContextInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContext<DBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
}
