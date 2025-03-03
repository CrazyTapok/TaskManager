using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Tests.Controllers;

public class CompanyControllerTests
{
    private readonly Mock<IService<Company>> _mockCompanyService;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly CompanyController _controller;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public CompanyControllerTests()
    {
        _mockCompanyService = new Mock<IService<Company>>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        _controller = new CompanyController(_mockCompanyService.Object, _mockEmployeeService.Object);
        _fixture = new Fixture();
        _cancellationToken = new CancellationToken();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsOkResult_WhenCompanyExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = _fixture.Build<Company>().With(company => company.Id, companyId).Create();
        _mockCompanyService.Setup(service => service.GetByIdAsync(companyId, _cancellationToken))
                           .ReturnsAsync(company);

        // Act
        var result = await _controller.GetCompanyByIdAsync(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var companyResponse = Assert.IsType<CompanyResponse>(okResult.Value);
        Assert.Equal(companyId, companyResponse.Id);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsNotFoundResult_WhenCompanyDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _mockCompanyService.Setup(service => service.GetByIdAsync(companyId, _cancellationToken))
                           .ReturnsAsync((Company)null);

        // Act
        var result = await _controller.GetCompanyByIdAsync(companyId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddCompanyAsync_ReturnsCreatedAtActionResult_WhenCompanyIsCreated()
    {
        // Arrange
        var companyRequest = _fixture.Create<CompanyRequest>();
        var createdCompany = _fixture.Build<Company>().With(company => company.Title, companyRequest.Title).Create();
        _mockCompanyService.Setup(service => service.AddAsync(It.IsAny<Company>(), _cancellationToken))
                           .ReturnsAsync(createdCompany);

        // Act
        var result = await _controller.AddCompanyAsync(companyRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var companyResponse = Assert.IsType<CompanyResponse>(createdAtActionResult.Value);
        Assert.Equal(createdCompany.Id, companyResponse.Id);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ReturnsNoContentResult_WhenCompanyIsUpdated()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var companyRequest = _fixture.Create<CompanyRequest>();
        _mockCompanyService.Setup(service => service.UpdateAsync(It.IsAny<Company>(), _cancellationToken))
                           .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateCompanyAsync(companyId, companyRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ReturnsNotFoundResult_WhenCompanyDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var companyRequest = _fixture.Create<CompanyRequest>();
        _mockCompanyService.Setup(service => service.UpdateAsync(It.IsAny<Company>(), _cancellationToken))
                           .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateCompanyAsync(companyId, companyRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCompanyAsync_ReturnsNoContentResult_WhenCompanyIsDeleted()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _mockCompanyService.Setup(service => service.DeleteAsync(companyId, _cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCompanyAsync(companyId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetEmployeesByCompanyIdAsync_ReturnsOkResult_WithEmployees()
    {
        // Arrange
        var expectedCount = 2;
        var companyId = Guid.NewGuid();
        var employees = _fixture.CreateMany<Employee>(expectedCount).ToList();
        _mockEmployeeService.Setup(service => service.GetEmployeesByCompanyIdAsync(companyId, _cancellationToken))
                            .ReturnsAsync(employees);

        // Act
        var result = await _controller.GetEmployeesByCompanyIdAsync(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var employeeResponses = Assert.IsType<List<EmployeeResponse>>(okResult.Value);
        Assert.Equal(expectedCount, employeeResponses.Count);
    }

    [Fact]
    public async Task ListAllCompaniesAsync_ReturnsOkResult_WithCompanies()
    {
        // Arrange
        var expectedCount = 2;
        var companies = _fixture.CreateMany<Company>(expectedCount).ToList();
        _mockCompanyService.Setup(service => service.ListAllAsync(_cancellationToken))
                           .ReturnsAsync(companies);

        // Act
        var result = await _controller.ListAllCompaniesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var companyResponses = Assert.IsType<List<CompanyResponse>>(okResult.Value);
        Assert.Equal(expectedCount, companyResponses.Count);
    }
}
