using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class EmployeeRepositoryTests
{
    private readonly DbContextOptions<DBContext> _options;
    private readonly TestDBContext _context;
    private readonly EmployeeRepository _employeeRepository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public EmployeeRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _employeeRepository = new EmployeeRepository(_context);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEmployees()
    {
        // Arrange
        var employeeName = "BestEmployee";
        var employees = _fixture.CreateMany<Employee>().ToList();
        await _context.Set<Employee>().AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        Expression<Func<Employee, bool>> predicate = employee => employee.Name.Contains(employeeName);
        var matchingEmployees = employees.Where(predicate.Compile()).ToList();

        // Act
        var result = await _employeeRepository.FindAsync(predicate, _cancellationToken);

        // Assert
        Assert.Equal(matchingEmployees.Count, result.Count);
        Assert.All(result, employee => Assert.Contains(employeeName, employee.Name));
    }
}
