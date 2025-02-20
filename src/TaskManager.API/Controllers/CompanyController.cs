using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Request;
using TaskManager.API.Contracts.Response;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private readonly IService<Company> _companyService;

    public CompanyController(IService<Company> companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _companyService.GetByIdAsync(id, cancellationToken);
       
        if (company == null)
        {
            return NotFound();
        }

        var response = new CompanyResponse
        {
            Id = company.Id,
            Title = company.Title,
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyResponse>> AddCompanyAsync([FromBody] CompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = new Company
        {
            Title = request.Title,
        };

        var createdCompany = await _companyService.AddAsync(company, cancellationToken);
       
        var response = new CompanyResponse
        {
            Id = createdCompany.Id,
            Title = createdCompany.Title,
        };

        return CreatedAtAction(nameof(GetCompanyByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompanyAsync(Guid id, [FromBody] CompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = new Company
        {
            Id = id,
            Title = request.Title,
        };

        var result = await _companyService.UpdateAsync(company, cancellationToken);
       
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompanyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _companyService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyResponse>>> ListAllCompaniesAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _companyService.ListAllAsync(cancellationToken);
        var response = companies.Select(company => new CompanyResponse
        {
            Id = company.Id,
            Title = company.Title,
        }).ToList();

        return Ok(response);
    }
}
