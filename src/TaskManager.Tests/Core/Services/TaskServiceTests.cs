using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockRepo;
        private readonly TaskService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public TaskServiceTests()
        {
            _mockRepo = new Mock<ITaskRepository>();
            _service = new TaskService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetByIdAsync()
        {
            var task = _fixture.Create<TaskManager.Core.Models.Task>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(task.Id, _cancellationToken)).ReturnsAsync(task);

            var result = await _service.GetByIdAsync(task.Id, _cancellationToken);

            Assert.Equal(task, result);
        }

        [Fact]
        public async Task GetByIdAsync_Returnstask()
        {
            var taskId = _fixture.Create<Guid>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(taskId, _cancellationToken)).ReturnsAsync((TaskManager.Core.Models.Task)null);

            await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(taskId, _cancellationToken));

        }
        [Fact]
        public async Task ListAllAsync_ThrowsValidationException()
        {
            var tasks = _fixture.CreateMany<TaskManager.Core.Models.Task>().ToList();
            _mockRepo.Setup(repo => repo.GetAllAsync(_cancellationToken)).ReturnsAsync(tasks);

            var result = await _service.ListAllAsync(_cancellationToken);

            Assert.Equal(tasks, result);
        }

        [Fact]
        public async Task AddAsync()
        {
            var task = _fixture.Create<TaskManager.Core.Models.Task>();
            _mockRepo.Setup(repo => repo.AddAsync(task, _cancellationToken)).ReturnsAsync(task);

            var result = await _service.AddAsync(task, _cancellationToken);

            Assert.Equal(task, result);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var task = _fixture.Create<TaskManager.Core.Models.Task>();
            _mockRepo.Setup(repo => repo.UpdateAsync(task, _cancellationToken)).ReturnsAsync(true);

            var result = await _service.UpdateAsync(task, _cancellationToken);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            var task = _fixture.Create<TaskManager.Core.Models.Task>();
            _mockRepo.Setup(repo => repo.DeleteAsync(task.Id, _cancellationToken))
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(task, _cancellationToken);

            _mockRepo.Verify(repo => repo.DeleteAsync(task.Id, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync()
        {
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskManager.Core.Models.Task>
            {
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task1", ProjectId = projectId },
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task2", ProjectId = projectId },
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task3", ProjectId = projectId }
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<TaskManager.Core.Models.Task, bool>>>(), _cancellationToken))
               .ReturnsAsync((Expression<Func<TaskManager.Core.Models.Task, bool>> predicate, CancellationToken token) =>
               {
                   return tasks.Where(predicate.Compile()).ToList();
               });

            var result = await _service.GetTasksByProjectIdAsync(projectId, _cancellationToken);

            Assert.Equal(tasks.Count, result.Count);
            Assert.All(result, task => Assert.Contains(task, tasks));
        }

        [Fact]
        public async Task GetTasksByEmployeeIdAsync()
        {
            var employeeId = Guid.NewGuid();
            var tasks = new List<TaskManager.Core.Models.Task>
            {
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task1", AssinedEmployeeId = employeeId },
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task2", AssinedEmployeeId = Guid.NewGuid() },
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task3", CreateEmployeeId = employeeId }
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<TaskManager.Core.Models.Task, bool>>>(), _cancellationToken))
                 .ReturnsAsync((Expression<Func<TaskManager.Core.Models.Task, bool>> predicate, CancellationToken token) =>
                 {
                     return tasks.Where(predicate.Compile()).ToList();
                 });

            var result = await _service.GetTasksByEmployeeIdAsync(employeeId, _cancellationToken);

            Assert.Equal(2, result.Count);
            Assert.All(result, task => Assert.Contains(task, tasks));
        }
    }
}
