using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using TaskManager.Tests.TestModels;

namespace TaskManager.Tests.Infrastructure;

public class RepositoryTests
{
    private readonly DbContextOptions<TestDBContext> _options;
    private readonly TestDBContext _context;
    private readonly IRepository<TestModel> _repository;
    private readonly Fixture _fixture;

    public RepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TestDBContext>()
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

        await _repository.AddAsync(entity);

        var addedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.NotNull(addedEntity);
    }

    [Fact]
    public async Task GetAllEntities()
    {
        var entities = _fixture.CreateMany<TestModel>().ToList();
        await _context.Set<TestModel>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(entities.Count, result.Count);
    }

    [Fact]
    public async Task GetEntityById()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);

        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        entity.Title = "Updated Name";
        await _repository.UpdateAsync(entity);

        var updatedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.Equal("Updated Name", updatedEntity.Title);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(entity.Id);

        var deletedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.Null(deletedEntity);
    }
}
