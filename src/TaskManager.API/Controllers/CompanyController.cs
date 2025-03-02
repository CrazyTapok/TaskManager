using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(IService<Company> companyService) : ControllerBase
{
    private readonly IService<Company> _companyService = companyService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> GetCompanyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _companyService.GetByIdAsync(id, cancellationToken);
        if (company == null)
        {
            return NotFound();
        }

        var response = company.MapToCompanyResponse();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyResponse>> AddCompanyAsync([FromBody] CompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = request.MapToCompany();
        var createdCompany = await _companyService.AddAsync(company, cancellationToken);

        var response = createdCompany.MapToCompanyResponse();

        return CreatedAtAction(nameof(GetCompanyByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompanyAsync(Guid id, [FromBody] CompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = request.MapToCompany(id);
        var updateSuccessful = await _companyService.UpdateAsync(company, cancellationToken);
       
        if (!updateSuccessful)
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
        var response = companies.Select(company => company.MapToCompanyResponse()).ToList();

        return Ok(response);
    }
}
