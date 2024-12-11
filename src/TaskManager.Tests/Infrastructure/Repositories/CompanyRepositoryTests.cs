using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class CompanyRepositoryTests
{
    private readonly DbContextOptions<TestDBContext> _options;
    private readonly TestDBContext _context;
    private readonly ICompanyRepository _companyRepository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CompanyRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TestDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _companyRepository = new CompanyRepository(_context);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingCompanies()
    {
        var companies = _fixture.CreateMany<Company>().ToList();
        await _context.Set<Company>().AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        Expression<Func<Company, bool>> predicate = c => c.Title.Contains("BestSoft");
        var matchingCompanies = companies.Where(predicate.Compile()).ToList();

        var result = await _companyRepository.FindAsync(predicate, _cancellationToken);

        Assert.Equal(matchingCompanies.Count, result.Count());
        Assert.All(result, company => Assert.Contains("BestSoft", company.Title));
    }
}
