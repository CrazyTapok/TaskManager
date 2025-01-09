using AutoFixture;
using Moq;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IRepository<Employee>> _mockRepo;
        private readonly EmployeeService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public EmployeeServiceTests()
        {
            _mockRepo = new Mock<IRepository<Employee>>();
            _service = new EmployeeService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetEmployeesByProjectIdAsync()
        {
            // Arrange
            var expectedCount = 2;
            var projectId = Guid.NewGuid();
            var employees = new List<Employee>
            {
                _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, projectId).Create()]).Create(),
                _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, projectId).Create()]).Create(),
                _fixture.Build<Employee>().With(employee => employee.Projects, [_fixture.Build<Project>().With(project => project.Id, Guid.NewGuid).Create()]).Create()
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => employees.Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetEmployeesByProjectIdAsync(projectId, _cancellationToken);

            // Assert
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, employee => Assert.Contains(employee, employees));
        }

        [Fact] 
        public async Task GetEmployeesByCompanyIdAsync() 
        {
            // Arrange
            var expectedCount = 2;
            var companyId = Guid.NewGuid(); 
            var employees = new List<Employee> 
            {
               
                _fixture.Build<Employee>().With(employee => employee.CompanyId, companyId).Create(),
                _fixture.Build<Employee>().With(employee => employee.CompanyId, companyId).Create(),
                _fixture.Build<Employee>().With(employee => employee.CompanyId, Guid.NewGuid).Create()
            }; 
            
            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => employees.Where(predicate.Compile()).ToList());

            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(companyId, _cancellationToken);

            // Assert
            Assert.Equal(expectedCount, result.Count); 
            Assert.All(result, employee => Assert.Contains(employee, employees)); 
        }
    }
}
