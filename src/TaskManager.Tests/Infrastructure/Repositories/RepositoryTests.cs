using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using TaskManager.Tests.TestModels;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class RepositoryTests
{
    private readonly DbContextOptions<DBContext> _options;
    private readonly TestDBContext _context;
    private readonly IRepository<TestModel> _repository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public RepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _repository = new Repository<TestModel>(_context);
    }

    [Fact]
    public async Task AddEntity()
    {
        var entity = _fixture.Create<TestModel>();

        await _repository.AddAsync(entity, _cancellationToken);

        var addedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.NotNull(addedEntity);
    }

    [Fact]
    public async Task GetAllEntities()
    {
        var entities = _fixture.CreateMany<TestModel>().ToList();
        await _context.Set<TestModel>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(_cancellationToken);

        Assert.Equal(entities.Count, result.Count);
    }

    [Fact]
    public async Task GetEntityById()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id, _cancellationToken);

        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        entity.Title = "Updated Name";
        await _repository.UpdateAsync(entity, _cancellationToken);

        var updatedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.Equal("Updated Name", updatedEntity.Title);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(entity.Id, _cancellationToken);

        var deletedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.NotNull(deletedEntity);
        Assert.True(deletedEntity.IsDeleted);
    }
}
