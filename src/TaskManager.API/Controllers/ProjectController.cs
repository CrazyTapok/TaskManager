using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services.ProjectManagement;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService, IEmployeeService employeeService, ITaskService taskService) : ControllerBase
{

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await projectService.GetByIdAsync(id, cancellationToken);
        var response = project.MapToProjectResponse();

        return Ok(response);
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> AddProjectAsync([FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = request.ToProject();
        var createdProject = await projectService.AddAsync(project, cancellationToken);

        var response = createdProject.MapToProjectResponse();

        return CreatedAtAction(nameof(GetProjectByIdAsync), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectAsync(Guid id, [FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = request.ToProject(id);
        var updateSuccessful = await projectService.UpdateAsync(project, cancellationToken);

        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await projectService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{projectId:guid}/employees")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var employees = await employeeService.GetEmployeesByProjectIdAsync(projectId, cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{projectId:guid}/tasks")]
    public async Task<ActionResult<List<TaskResponse>>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await taskService.GetTasksByProjectIdAsync(projectId, cancellationToken);
        var response = tasks.Select(task => task.MapToTaskResponse()).ToList();

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> ListAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await projectService.ListAllAsync(cancellationToken);
        var response = projects.Select(project => project.MapToProjectResponse()).ToList();

        return Ok(response);
    }
}