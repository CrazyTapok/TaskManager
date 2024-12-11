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
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockRepo;
        private readonly ProjectService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public ProjectServiceTests()
        {
            _mockRepo = new Mock<IProjectRepository>();
            _service = new ProjectService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetByIdAsync()
        {
            var project = _fixture.Create<Project>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(project.Id, _cancellationToken)).ReturnsAsync(project);

            var result = await _service.GetByIdAsync(project.Id, _cancellationToken);

            Assert.Equal(project, result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProject()
        {
            var projectId = _fixture.Create<Guid>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(projectId, _cancellationToken)).ReturnsAsync((Project)null);

            await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(projectId, _cancellationToken));

        }
        [Fact]
        public async Task ListAllAsync_ThrowsValidationException()
        {
            var projects = _fixture.CreateMany<Project>().ToList();
            _mockRepo.Setup(repo => repo.GetAllAsync(_cancellationToken)).ReturnsAsync(projects);

            var result = await _service.ListAllAsync(_cancellationToken);

            Assert.Equal(projects, result);
        }

        [Fact]
        public async Task AddAsync()
        {
            var project = _fixture.Create<Project>();
            _mockRepo.Setup(repo => repo.AddAsync(project, _cancellationToken)).ReturnsAsync(project);

            var result = await _service.AddAsync(project, _cancellationToken);

            Assert.Equal(project, result);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var project = _fixture.Create<Project>();
            _mockRepo.Setup(repo => repo.UpdateAsync(project, _cancellationToken)).ReturnsAsync(true);

            var result = await _service.UpdateAsync(project, _cancellationToken);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            var project = _fixture.Create<Project>();
            _mockRepo.Setup(repo => repo.DeleteAsync(project.Id, _cancellationToken))
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(project, _cancellationToken);

            _mockRepo.Verify(repo => repo.DeleteAsync(project.Id, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetProjectsByEmployeeIdAsync()
        {
            var employeeId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Employees = new List<Employee> { new Employee { Id = employeeId } } },
                new Project { Id = Guid.NewGuid(), Employees = new List<Employee> { new Employee { Id = employeeId } } },
                new Project { Id = Guid.NewGuid(), Employees = new List<Employee> { new Employee { Id = Guid.NewGuid() } } }
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Project, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Project, bool>> predicate, CancellationToken token) =>
                {
                    return projects.Where(predicate.Compile()).ToList();
                });

            var result = await _service.GetProjectsByEmployeeIdAsync(employeeId, _cancellationToken);

            Assert.Equal(2, result.Count);
            Assert.All(result, project => Assert.Contains(project, projects));
        }
    }
}
