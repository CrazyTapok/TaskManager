using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Extensions;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            return NotFound();
        }

        var response = task.MapToTaskResponse();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> AddAsync([FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = request.ToTask();
        var createdTask = await _taskService.AddAsync(task, cancellationToken);
        
        var response = createdTask.MapToTaskResponse();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = request.ToTask(id);
        var updateSuccessful = await _taskService.UpdateAsync(task, cancellationToken);
      
        if (!updateSuccessful)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> ListAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.ListAllAsync(cancellationToken);
        var response = tasks.Select(task => task.MapToTaskResponse()).ToList();

        return Ok(response);
    }
}