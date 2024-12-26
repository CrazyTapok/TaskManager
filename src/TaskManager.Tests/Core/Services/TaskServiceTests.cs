using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Services;

namespace TaskManager.Tests.Core.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IRepository<TaskManager.Core.Models.Task>> _mockRepo;
        private readonly TaskService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public TaskServiceTests()
        {
            _mockRepo = new Mock<IRepository<TaskManager.Core.Models.Task>>();
            _service = new TaskService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
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
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task1", AssignedEmployeeId = employeeId },
                new TaskManager.Core.Models.Task { Id = Guid.NewGuid(), Title = "Task2", AssignedEmployeeId = Guid.NewGuid() },
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
