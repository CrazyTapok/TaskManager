using AutoFixture;
using Moq;
using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Services;
using TaskManager.Tests.TestModels;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services;

public class ServiceTests
{
    private readonly Mock<IRepository<TestModel>> _repositoryMock;
    private readonly Service<TestModel> _service;
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
        // Arrange
        var employee = _fixture.Create<TestModel>();

        _repositoryMock.Setup(repository => repository.GetByIdAsync(employee.Id, _cancellationToken))
            .Returns(new ValueTask<TestModel?>(employee));

        // Act
        var result = await _service.GetByIdAsync(employee.Id, _cancellationToken);

        // Assert
        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid(); 
       
        _repositoryMock.Setup(repository => repository.GetByIdAsync(id, _cancellationToken))
            .Returns(new ValueTask<TestModel?>((TestModel)null));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id, _cancellationToken));
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var employees = _fixture.CreateMany<TestModel>().ToList();
      
        _repositoryMock.Setup(repository => repository.GetAllAsync(_cancellationToken))
            .ReturnsAsync(employees);

        // Act
        var result = await _service.ListAllAsync(_cancellationToken);

        // Assert
        Assert.Equal(employees.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        // Arrange
        var employee = _fixture.Create<TestModel>();
     
        _repositoryMock.Setup(repository => repository.AddAsync(employee, _cancellationToken))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.AddAsync(employee, _cancellationToken);

        // Assert
        Assert.Equal(employee, result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        // Arrange
        var employee = _fixture.Create<TestModel>();
       
        _repositoryMock.Setup(repository => repository.UpdateAsync(employee, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(employee, _cancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_DeletesEntity()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock.Setup(repository => repository.DeleteAsync(id, _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id, _cancellationToken);

        // Assert
        _repositoryMock.Verify(repository => repository.DeleteAsync(id, _cancellationToken), Times.Once);
    }
}
