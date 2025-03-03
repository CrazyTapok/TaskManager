using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService, IEmployeeService employeeService, ITaskService taskService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly ITaskService _taskService = taskService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound();
        }

        var response = project.MapToProjectResponse();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> AddProjectAsync([FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = request.ToProject();
        var createdProject = await _projectService.AddAsync(project, cancellationToken);

        var response = createdProject.MapToProjectResponse();

        return CreatedAtAction(nameof(GetProjectByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectAsync(Guid id, [FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = request.ToProject(id);
        var updateSuccessful = await _projectService.UpdateAsync(project, cancellationToken);

        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _projectService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("projects/{projectId:guid}/employees")]
    public async Task<ActionResult<List<EmployeeResponse>>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService.GetEmployeesByProjectIdAsync(projectId, cancellationToken);
        var response = employees.Select(employee => employee.MapToEmployeeResponse()).ToList();

        return Ok(response);
    }

    [HttpGet("projects/{projectId:guid}/tasks")]
    public async Task<ActionResult<List<TaskResponse>>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetTasksByProjectIdAsync(projectId, cancellationToken);
        var response = tasks.Select(task => task.MapToTaskResponse()).ToList();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> ListAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _projectService.ListAllAsync(cancellationToken);
        var response = projects.Select(project => project.MapToProjectResponse()).ToList();

        return Ok(response);
    }
}