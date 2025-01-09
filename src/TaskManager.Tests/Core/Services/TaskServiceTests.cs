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
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetTasksByProjectIdAsync()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskManager.Core.Models.Task>
            {
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.ProjectId, projectId).Create(),
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.ProjectId, projectId).Create(),
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.ProjectId, projectId).Create()
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<TaskManager.Core.Models.Task, bool>>>(), _cancellationToken))
               .ReturnsAsync((Expression<Func<TaskManager.Core.Models.Task, bool>> predicate, CancellationToken token) => tasks.Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetTasksByProjectIdAsync(projectId, _cancellationToken);

            // Assert
            Assert.Equal(tasks.Count, result.Count);
            Assert.All(result, task => Assert.Contains(task, tasks));
        }

        [Fact]
        public async Task GetTasksByEmployeeIdAsync()
        {
            // Arrange
            var expectedCount = 2;
            var employeeId = Guid.NewGuid();
            var tasks = new List<TaskManager.Core.Models.Task>
            {
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.AssignedEmployeeId, employeeId).Create(),
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.AssignedEmployeeId, Guid.NewGuid()).Create(),
                _fixture.Build<TaskManager.Core.Models.Task>().With(task => task.CreateEmployeeId, employeeId).Create()
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<TaskManager.Core.Models.Task, bool>>>(), _cancellationToken))
                 .ReturnsAsync((Expression<Func<TaskManager.Core.Models.Task, bool>> predicate, CancellationToken token) => tasks.Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetTasksByEmployeeIdAsync(employeeId, _cancellationToken);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, task => Assert.Contains(task, tasks));
        }
    }
}
