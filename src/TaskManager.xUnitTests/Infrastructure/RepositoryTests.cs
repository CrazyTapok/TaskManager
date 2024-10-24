using Moq;
using TaskManager.Core.Interfaces;

namespace TaskManager.xUnitTests.Infrastructure;
public class RepositoryTests
{
    private readonly Mock<IRepository<TestModel>> _repository;

    public RepositoryTests()
    {
        _repository = new Mock<IRepository<TestModel>>();
    }

    [Fact]
    public async Task AddEntity_ShouldCallAddAsync()
    {
        var entity = new TestModel { Id = Guid.NewGuid(), Title = "Test 66" };

        await _repository.Object.AddAsync(entity);

        _repository.Verify(r => r.AddAsync(It.IsAny<TestModel>()), Times.Once);
    }

    [Fact]
    public async Task GetAllEntities_ShouldCallGetAllAsync()
    {
        var result = await _repository.Object.GetAllAsync();

        _repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEntityById_ShouldCallGetByIdAsync()
    {
        var id = Guid.NewGuid();

        var result = await _repository.Object.GetByIdAsync(id);

        _repository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEntity_ShouldCallUpdateAsync()
    {
        var entity = new TestModel { Id = Guid.NewGuid(), Title = "Updated Entity" };

        await _repository.Object.UpdateAsync(entity);

        _repository.Verify(r => r.UpdateAsync(It.IsAny<TestModel>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEntity_ShouldCallDeleteAsync()
    {
        var id = Guid.NewGuid();

        await _repository.Object.DeleteAsync(id);

        _repository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Once);
    }


    public class TestModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }
}
