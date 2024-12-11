using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class EmployeeRepositoryTests
{
    private readonly DbContextOptions<TestDBContext> _options;
    private readonly TestDBContext _context;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public EmployeeRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TestDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _employeeRepository = new EmployeeRepository(_context);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEmployees()
    {
        var employees = _fixture.CreateMany<Employee>().ToList();
        await _context.Set<Employee>().AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        Expression<Func<Employee, bool>> predicate = t => t.Name.Contains("BestEmployee");
        var matchingEmployees = employees.Where(predicate.Compile()).ToList();

        var result = await _employeeRepository.FindAsync(predicate, _cancellationToken);

        Assert.Equal(matchingEmployees.Count, result.Count());
        Assert.All(result, employee => Assert.Contains("BestEmployee", employee.Name));
    }
}
