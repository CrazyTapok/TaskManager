using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;

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

    [HttpGet("employees/{employeeId:guid}/projects")]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectService.GetProjectsByEmployeeIdAsync(employeeId, cancellationToken);
        var response = projects.Select(project => project.MapToProjectResponse()).ToList();

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