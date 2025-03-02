using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            return NotFound();
        }

        var response = employee.MapToEmployeeResponse();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = request.ToEmployee();
        var createdEmployee = await _employeeService.AddAsync(employee, cancellationToken);
        
        var response = createdEmployee.MapToEmployeeResponse();

        return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = request.ToEmployee(id);
        var updateSuccessful = await _employeeService.UpdateAsync(employee, cancellationToken);
        
        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("projects/{projectId:guid}/employees")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesByProjectIdAsync(projectId, cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }

    [HttpGet("companies/{companyId:guid}/employees")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesByCompanyIdAsync(companyId, cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> ListAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.ListAllAsync(cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }
}