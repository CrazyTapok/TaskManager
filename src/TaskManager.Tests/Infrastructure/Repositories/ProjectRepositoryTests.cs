using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class ProjectRepositoryTests
{
    private readonly DbContextOptions<DBContext> _options;
    private readonly TestDBContext _context;
    private readonly ProjectRepository _projectRepository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ProjectRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _projectRepository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingProjects()
    {
        // Arrange
        var projectTitle = "BestProject";
        var projects = _fixture.CreateMany<Project>().ToList();
        await _context.Set<Project>().AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        Expression<Func<Project, bool>> predicate = project => project.Title.Contains(projectTitle);
        var matchingProjects = projects.Where(predicate.Compile()).ToList();

        // Act
        var result = await _projectRepository.FindAsync(predicate, _cancellationToken);

        // Assert
        Assert.Equal(matchingProjects.Count, result.Count);
        Assert.All(result, project => Assert.Contains(projectTitle, project.Title));
    }
}
