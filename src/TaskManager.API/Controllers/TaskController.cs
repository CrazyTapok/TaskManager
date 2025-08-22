using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services.ProjectManagement;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService) : ControllerBase
{

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await taskService.GetByIdAsync(id, cancellationToken);
        var response = task.MapToTaskResponse();
       
        return Ok(response);
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> AddAsync([FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = request.ToTask();
        var createdTask = await taskService.AddAsync(task, cancellationToken);
        
        var response = createdTask.MapToTaskResponse();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [Authorize(Roles = "Admin,ProjectManager,Developer")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = request.ToTask(id);
        var updateSuccessful = await taskService.UpdateAsync(task, cancellationToken);
      
        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> ListAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await taskService.ListAllAsync(cancellationToken);
        var response = tasks.Select(task => task.MapToTaskResponse()).ToList();

        return Ok(response);
    }
}