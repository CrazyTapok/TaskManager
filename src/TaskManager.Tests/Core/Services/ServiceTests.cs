using AutoFixture;
using Moq;
using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Services;
using TaskManager.Tests.TestModels;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class ServiceTests
{
    private readonly Mock<IRepository<TestModel>> _repositoryMock;
    private readonly IService<TestModel> _service;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ServiceTests()
    {
        _repositoryMock = new Mock<IRepository<TestModel>>();
        _service = new Service<TestModel>(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
    {
        var employee = _fixture.Create<TestModel>();
        _repositoryMock.Setup(r => r.GetByIdAsync(employee.Id, _cancellationToken))
            .Returns(new ValueTask<TestModel>(employee));

        var result = await _service.GetByIdAsync(employee.Id, _cancellationToken);

        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        var id = Guid.NewGuid(); 
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id, _cancellationToken))
            .Returns(new ValueTask<TestModel>((TestModel)null)); 
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id, _cancellationToken));
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllEntities()
    {
        var employees = _fixture.CreateMany<TestModel>().ToList();
        _repositoryMock.Setup(r => r.GetAllAsync(_cancellationToken))
            .ReturnsAsync(employees);

        var result = await _service.ListAllAsync(_cancellationToken);

        Assert.Equal(employees.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        var employee = _fixture.Create<TestModel>();
        _repositoryMock.Setup(r => r.AddAsync(employee, _cancellationToken))
            .ReturnsAsync(employee);

        var result = await _service.AddAsync(employee, _cancellationToken);

        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var employee = _fixture.Create<TestModel>();
        _repositoryMock.Setup(r => r.UpdateAsync(employee, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(employee, _cancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntity()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeleteAsync(id, _cancellationToken))
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(id, _cancellationToken);

        _repositoryMock.Verify(r => r.DeleteAsync(id, _cancellationToken), Times.Once);
    }
}
