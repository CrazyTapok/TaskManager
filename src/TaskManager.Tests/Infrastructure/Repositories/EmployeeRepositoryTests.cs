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

    [Fact]
    public async Task GetByEmailAsync_ReturnsCorrectEmployee()
    {
        // Arrange
        var employeeEmail = "test@example.com";
        var employee = _fixture.Build<Employee>().With(employee => employee.Email, employeeEmail).Create();

        await _context.Set<Employee>().AddAsync(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetByEmailAsync(employeeEmail, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeEmail, result!.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNullForNonExistingEmail()
    {
        // Arrange
        var nonExistingEmail = "nonexisting@example.com";
        var employees = _fixture.CreateMany<Employee>().ToList();
        await _context.Set<Employee>().AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetByEmailAsync(nonExistingEmail, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNewsletterEnabledEmployeesAsync_ReturnsOnlyEnabledEmployees()
    {
        // Arrange
        _context.Set<Employee>().RemoveRange(_context.Set<Employee>());
        await _context.SaveChangesAsync();

        var enabledEmployees = _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, true).CreateMany(2).ToList();
        var disabledEmployees = _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, false).CreateMany(2).ToList();

        await _context.Set<Employee>().AddRangeAsync(enabledEmployees);
        await _context.Set<Employee>().AddRangeAsync(disabledEmployees);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetNewsletterEnabledEmployeesAsync(_cancellationToken);

        // Assert
        Assert.Equal(enabledEmployees.Count, result.Count);
        Assert.All(result, employee => Assert.True(employee.IsDailyNewsletterEnabled));
    }

    [Fact]
    public async Task GetNewsletterEnabledEmployeesAsync_ReturnsEmptyListWhenNoEnabledEmployees()
    {
        // Arrange
        _context.Set<Employee>().RemoveRange(_context.Set<Employee>());
        await _context.SaveChangesAsync();

        var disabledEmployees = _fixture.Build<Employee>().With(employee => employee.IsDailyNewsletterEnabled, false).CreateMany(3).ToList();
        await _context.Set<Employee>().AddRangeAsync(disabledEmployees);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetNewsletterEnabledEmployeesAsync(_cancellationToken);

        // Assert
        Assert.Empty(result);
    }
}
