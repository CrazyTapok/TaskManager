using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Infrastructure.EF;

internal class DBContext : DbContext
{
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
        if (Database.IsRelational())
        {
            if (Database.GetPendingMigrations().Any())
                Database.Migrate();
            else if (!Database.CanConnect())
                Database.EnsureCreated();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetAssembly(typeof(DBContext));

        if (assembly != null)
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(modelBuilder);
    }
}
