using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services.ProjectManagement;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeeController(IEmployeeService employeeService, IProjectService projectService, ITaskService taskService) : ControllerBase
{

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeService.GetByIdAsync(id, cancellationToken);
        var response = employee.MapToEmployeeResponse();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var employee = request.ToEmployee();
            var createdEmployee = await employeeService.RegisterAsync(employee, cancellationToken);

            var response = createdEmployee.MapToEmployeeResponse();

            return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = request.ToEmployee(id);
        var updateSuccessful = await employeeService.UpdateAsync(employee, cancellationToken);
        
        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{employeeId:guid}/projects")]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var projects = await projectService.GetProjectsByEmployeeIdAsync(employeeId, cancellationToken);
        var response = projects.Select(project => project.MapToProjectResponse()).ToList();

        return Ok(response);
    }

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{employeeId:guid}/tasks")]
    public async Task<ActionResult<List<TaskResponse>>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var tasks = await taskService.GetTasksByEmployeeIdAsync(employeeId, cancellationToken);
        var response = tasks.Select(task => task.MapToTaskResponse()).ToList();

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> ListAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeService.ListAllAsync(cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }
}