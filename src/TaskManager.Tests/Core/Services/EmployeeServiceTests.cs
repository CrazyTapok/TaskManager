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
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeeService _service;
        private readonly CancellationToken _cancellationToken;
        private readonly Fixture _fixture;

        public EmployeeServiceTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _service = new EmployeeService(_mockRepo.Object);
            _cancellationToken = new CancellationToken();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetByIdAsync()
        {
            var employee = _fixture.Create<Employee>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(employee.Id, _cancellationToken)).ReturnsAsync(employee);

            var result = await _service.GetByIdAsync(employee.Id, _cancellationToken);

            Assert.Equal(employee, result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEmployee()
        {
            var employeeId = _fixture.Create<Guid>();
            _mockRepo.Setup(repo => repo.GetByIdAsync(employeeId, _cancellationToken)).ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(employeeId, _cancellationToken));

        }
        [Fact]
        public async Task ListAllAsync_ThrowsValidationException()
        {
            var employees = _fixture.CreateMany<Employee>().ToList();
            _mockRepo.Setup(repo => repo.GetAllAsync(_cancellationToken)).ReturnsAsync(employees);

            var result = await _service.ListAllAsync(_cancellationToken);

            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task AddAsync()
        {
            var employee = _fixture.Create<Employee>();
            _mockRepo.Setup(repo => repo.AddAsync(employee, _cancellationToken)).ReturnsAsync(employee);

            var result = await _service.AddAsync(employee, _cancellationToken);

            Assert.Equal(employee, result);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var employee = _fixture.Create<Employee>();
            _mockRepo.Setup(repo => repo.UpdateAsync(employee, _cancellationToken)).ReturnsAsync(true);

            var result = await _service.UpdateAsync(employee, _cancellationToken);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            var employee = _fixture.Create<Employee>();
            _mockRepo.Setup(repo => repo.DeleteAsync(employee.Id, _cancellationToken))
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(employee, _cancellationToken);

            _mockRepo.Verify(repo => repo.DeleteAsync(employee.Id, _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetEmployeesByProjectIdAsync()
        {
            var projectId = Guid.NewGuid();

            var employees = new List<Employee>
            {
                new Employee { Id = Guid.NewGuid(), Name = "Employee1", Projects = new List<Project> { new Project { Id = projectId } } },
                new Employee { Id = Guid.NewGuid(), Name = "Employee2", Projects = new List<Project> { new Project { Id = projectId } } },
                new Employee { Id = Guid.NewGuid(), Name = "Employee3", Projects = new List<Project> { new Project { Id = Guid.NewGuid() } } }
            };

            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) =>
                {
                    return employees.Where(predicate.Compile()).ToList();
                });

            var result = await _service.GetEmployeesByProjectIdAsync(projectId, _cancellationToken);

            Assert.Equal(2, result.Count);
            Assert.All(result, employee => Assert.Contains(employee, employees));
        }

        [Fact] 
        public async Task GetEmployeesByCompanyIdAsync() 
        { 
            var companyId = Guid.NewGuid(); 
            var employees = new List<Employee> 
            { 
                new Employee { Id = Guid.NewGuid(), Name = "Employee1", CompanyId = companyId }, 
                new Employee { Id = Guid.NewGuid(), Name = "Employee2", CompanyId = companyId }, 
                new Employee { Id = Guid.NewGuid(), Name = "Employee3", CompanyId = Guid.NewGuid() } 
            }; 
            
            _mockRepo.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>(), _cancellationToken))
                .ReturnsAsync((Expression<Func<Employee, bool>> predicate, CancellationToken token) => 
                { 
                    return employees.Where(predicate.Compile()).ToList(); 
                }); 
            
            var result = await _service.GetEmployeesByCompanyIdAsync(companyId, _cancellationToken); 
            
            Assert.Equal(2, result.Count); 
            Assert.All(result, employee => Assert.Contains(employee, employees)); 
        }
    }
}
