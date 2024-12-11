using AutoFixture;
using Moq;
using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Tests.Core.Services
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> _mockRepo;
        private readonly CompanyService _service;
        private readonly CancellationToken _cancellationToken; 
        private readonly Fixture _fixture; 
        
        public CompanyServiceTests() 
        {
            _mockRepo = new Mock<ICompanyRepository>();
            _service = new CompanyService(_mockRepo.Object); 
            _cancellationToken = new CancellationToken(); 
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact] 
        public async Task GetByIdAsync() 
        { 
            var company = _fixture.Create<Company>(); 
            _mockRepo.Setup(repo => repo.GetByIdAsync(company.Id, _cancellationToken)).ReturnsAsync(company); 
            
            var result = await _service.GetByIdAsync(company.Id, _cancellationToken); 
            
            Assert.Equal(company, result); 
        }

        [Fact] 
        public async Task GetByIdAsync_ReturnsCompany() 
        { 
            var companyId = _fixture.Create<Guid>(); 
            _mockRepo.Setup(repo => repo.GetByIdAsync(companyId, _cancellationToken)).ReturnsAsync((Company)null); 
            
            await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(companyId, _cancellationToken)); 
        
        }
        [Fact] 
        public async Task ListAllAsync_ThrowsValidationException() 
        { 
            var companies = _fixture.CreateMany<Company>().ToList(); 
            _mockRepo.Setup(repo => repo.GetAllAsync(_cancellationToken)).ReturnsAsync(companies); 
            
            var result = await _service.ListAllAsync(_cancellationToken); 
            
            Assert.Equal(companies, result); 
        }

        [Fact] 
        public async Task AddAsync() 
        { 
            var company = _fixture.Create<Company>(); 
            _mockRepo.Setup(repo => repo.AddAsync(company, _cancellationToken)).ReturnsAsync(company); 
            
            var result = await _service.AddAsync(company, _cancellationToken); 
            
            Assert.Equal(company, result); 
        }

        [Fact] 
        public async Task UpdateAsync() 
        { 
            var company = _fixture.Create<Company>(); 
            _mockRepo.Setup(repo => repo.UpdateAsync(company, _cancellationToken)).ReturnsAsync(true); 
        
            var result = await _service.UpdateAsync(company, _cancellationToken); 
            
            Assert.True(result); 
        }

        [Fact]
        public async Task DeleteAsync()
        {
            var company = _fixture.Create<Company>();
            _mockRepo.Setup(repo => repo.DeleteAsync(company.Id, _cancellationToken))
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(company, _cancellationToken);

            _mockRepo.Verify(repo => repo.DeleteAsync(company.Id, _cancellationToken), Times.Once);
        }

    }
}
