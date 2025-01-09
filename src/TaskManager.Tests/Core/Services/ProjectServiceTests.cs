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
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetProjectsByEmployeeIdAsync()
        {
            // Arrange
            var expectedCount = 2;
            var employeeId = Guid.NewGuid();
            var projects = new List<Project>
            {
                _fixture.Build<Project>().With(project => project.Employees, [_fixture.Build<Employee>().With(employee => employee.Id, employeeId).Create()]).Create(),
                _fixture.Build<Project>().With(project => project.Employees, [_fixture.Build<Employee>().With(employee => employee.Id, employeeId).Create()]).Create(),
                _fixture.Build<Project>().With(project => project.Employees, [_fixture.Build<Employee>().With(employee => employee.Id, Guid.NewGuid).Create()]).Create()
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Project, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Project, bool>> predicate, CancellationToken token) => projects.Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetProjectsByEmployeeIdAsync(employeeId, _cancellationToken);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, project => Assert.Contains(project, projects));
        }
    }
}
