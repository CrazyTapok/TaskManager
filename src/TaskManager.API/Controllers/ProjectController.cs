using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Request;
using TaskManager.API.Contracts.Response;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return NotFound();
        }

        var response = new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            ManagerId = project.ManagerId,
            CompanyId = project.CompanyId,
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> AddProjectAsync([FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Title = request.Title,
            ManagerId = request.ManagerId,
            CompanyId = request.CompanyId,
        };

        var createdProject = await _projectService.AddAsync(project, cancellationToken);
       
        var response = new ProjectResponse
        {
            Id = createdProject.Id,
            Title = createdProject.Title,
            ManagerId = createdProject.ManagerId,
            CompanyId = createdProject.CompanyId,
        };

        return CreatedAtAction(nameof(GetProjectByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectAsync(Guid id, [FromBody] ProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Id = id,
            Title = request.Title,
            ManagerId = request.ManagerId,
            CompanyId = request.CompanyId,
        };

        var result = await _projectService.UpdateAsync(project, cancellationToken);
       
        if (!result)
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

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectService.GetProjectsByEmployeeIdAsync(employeeId, cancellationToken);
        var response = projects.Select(project => new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            ManagerId = project.ManagerId,
            CompanyId = project.CompanyId,
        }).ToList();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> ListAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _projectService.ListAllAsync(cancellationToken);
        var response = projects.Select(project => new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            ManagerId = project.ManagerId,
            CompanyId = project.CompanyId,
        }).ToList();

        return Ok(response);
    }
}