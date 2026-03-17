using AutoFixture;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;
using TaskManager.Tests.TestModels;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DBContext> _options;
    private readonly TestDBContext _context;
    private readonly Repository<TestModel> _repository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public RepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<DBContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TestDBContext(_options);

        // 3. Создаем схему таблиц в памяти
        _context.Database.EnsureCreated();

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _repository = new Repository<TestModel>(_context);
    }

    [Fact]
    public async Task AddEntity()
    {
        // Arrange
        var entity = _fixture.Create<TestModel>();

        // Act
        await _repository.AddAsync(entity, _cancellationToken);

        // Assert
        var addedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.NotNull(addedEntity);
    }

    [Fact]
    public async Task GetAllEntities()
    {
        // Arrange
        _context.Set<TestModel>().RemoveRange(_context.Set<TestModel>());
        await _context.SaveChangesAsync();

        var entities = _fixture.CreateMany<TestModel>().ToList();
        await _context.Set<TestModel>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(_cancellationToken);

        // Assert
        Assert.Equal(entities.Count, result.Count);
    }

    [Fact]
    public async Task GetEntityById()
    {
        // Arrange
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(entity.Id, _cancellationToken);

        // Assert
        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Arrange
        var title = "Title";
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Title = title;
        await _repository.UpdateAsync(entity, _cancellationToken);

        // Assert
        var updatedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.Equal(title, updatedEntity?.Title);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Arrange
        var entity = _fixture.Create<TestModel>();
        await _context.Set<TestModel>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity.Id, _cancellationToken);

        // Assert
        _context.ChangeTracker.Clear();

        var deletedEntity = await _context.Set<TestModel>().FindAsync(entity.Id);
        Assert.NotNull(deletedEntity);
        Assert.True(deletedEntity.IsDeleted);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose(); // Закрываем соединение, чтобы освободить память
    }
}
