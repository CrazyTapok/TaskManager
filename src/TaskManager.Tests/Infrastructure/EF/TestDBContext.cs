using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;
using TaskManager.Tests.TestModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Tests.Infrastructure.EF;

internal class TestDBContext : DBContext
{
    public DbSet<TestModel> TestModels { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;

    public TestDBContext(DbContextOptions<DBContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    { 
        var assembly = Assembly.GetAssembly(typeof(DBContext)); 
        
        if (assembly != null) 
        { 
            modelBuilder.ApplyConfigurationsFromAssembly(assembly); 
        } 
        
        base.OnModelCreating(modelBuilder); 
    }
}
