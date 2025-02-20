using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Request;
using TaskManager.API.Contracts.Response;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
       
        if (employee == null)
        {
            return NotFound();
        }

        var response = new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            CompanyId = employee.CompanyId,
            Role = employee.Role
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> AddEmployeeAsync([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = new Employee
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            CompanyId = request.CompanyId,
            Role = request.Role
        };

        var createdEmployee = await _employeeService.AddAsync(employee, cancellationToken);
        var response = new EmployeeResponse
        {
            Id = createdEmployee.Id,
            Name = createdEmployee.Name,
            Email = createdEmployee.Email,
            CompanyId = createdEmployee.CompanyId,
            Role = createdEmployee.Role
        };

        return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = new Employee
        {
            Id = id,
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            CompanyId = request.CompanyId,
            Role = request.Role
        };

        var result = await _employeeService.UpdateAsync(employee, cancellationToken);
        if (!result)
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

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesByProjectIdAsync(projectId, cancellationToken);
        var response = employees.Select(employee => new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            CompanyId = employee.CompanyId,
            Role = employee.Role
        }).ToList();

        return Ok(response);
    }

    [HttpGet("company/{companyId:guid}")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesByCompanyIdAsync(companyId, cancellationToken);
        var response = employees.Select(employee => new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            CompanyId = employee.CompanyId,
            Role = employee.Role
        }).ToList();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> ListAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.ListAllAsync(cancellationToken);
        var response = employees.Select(employee => new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            CompanyId = employee.CompanyId,
            Role = employee.Role
        }).ToList();

        return Ok(response);
    }
}