using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IRepository<Project>> _mockRepo;
        private readonly ProjectService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public ProjectServiceTests()
        {
            _mockRepo = new Mock<IRepository<Project>>();
            _service = new ProjectService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
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
