using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;
using TaskManager.Tests.TestModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Tests.Infrastructure.EF;

public class TestDBContext : DbContext
{
    public DbSet<TestModel> TestModels { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;


    public TestDBContext(DbContextOptions<TestDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(DBContext)));

        base.OnModelCreating(modelBuilder);
    }
}
