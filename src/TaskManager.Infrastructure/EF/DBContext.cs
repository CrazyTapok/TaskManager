using System.Reflection;
using TaskManager.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Task = TaskManager.Infrastructure.Entities.Task;

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
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(DBContext)));

        base.OnModelCreating(modelBuilder);
    }
}
