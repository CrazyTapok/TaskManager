using Microsoft.EntityFrameworkCore;
using TaskManager.Tests.TestModels;

namespace TaskManager.Tests.Infrastructure.EF;

public class TestDBContext : DbContext
{
    public TestDBContext(DbContextOptions<TestDBContext> options) : base(options) { }

    public DbSet<TestModel> TestModels { get; set; } = null!;
}
