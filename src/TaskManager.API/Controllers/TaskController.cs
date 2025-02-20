using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Request;
using TaskManager.API.Contracts.Response;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            return NotFound();
        }

        var response = new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            ProjectId = task.ProjectId,
            CreateEmployeeId = task.CreateEmployeeId,
            AssignedEmployeeId = task.AssignedEmployeeId,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate,
            Image = task.Image,
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> AddAsync([FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = new Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            ProjectId = request.ProjectId,
            CreateEmployeeId = request.CreateEmployeeId,
            AssignedEmployeeId = request.AssignedEmployeeId,
            CreatedDate = request.CreatedDate,
            UpdatedDate = request.UpdatedDate,
            Image = request.Image
        };

        var createdTask = await _taskService.AddAsync(task, cancellationToken);
        var response = new TaskResponse
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            Status = createdTask.Status,
            ProjectId = createdTask.ProjectId,
            CreateEmployeeId = createdTask.CreateEmployeeId,
            AssignedEmployeeId = createdTask.AssignedEmployeeId,
            CreatedDate = createdTask.CreatedDate,
            UpdatedDate = createdTask.UpdatedDate,
            Image = createdTask.Image,
        };

        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = new Task
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            ProjectId = request.ProjectId,
            CreateEmployeeId = request.CreateEmployeeId,
            AssignedEmployeeId = request.AssignedEmployeeId,
            CreatedDate = request.CreatedDate,
            UpdatedDate = request.UpdatedDate,
            Image = request.Image
        };

        var result = await _taskService.UpdateAsync(task, cancellationToken);
        if (!result)
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

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<List<TaskResponse>>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetTasksByProjectIdAsync(projectId, cancellationToken);
        var response = tasks.Select(task => new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            ProjectId = task.ProjectId,
            CreateEmployeeId = task.CreateEmployeeId,
            AssignedEmployeeId = task.AssignedEmployeeId,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate,
            Image = task.Image,
        }).ToList();

        return Ok(response);
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<List<TaskResponse>>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetTasksByEmployeeIdAsync(employeeId, cancellationToken);
        var response = tasks.Select(task => new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            ProjectId = task.ProjectId,
            CreateEmployeeId = task.CreateEmployeeId,
            AssignedEmployeeId = task.AssignedEmployeeId,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate,
            Image = task.Image,
        }).ToList();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> ListAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.ListAllAsync(cancellationToken);
        var response = tasks.Select(task => new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            ProjectId = task.ProjectId,
            CreateEmployeeId = task.CreateEmployeeId,
            AssignedEmployeeId = task.AssignedEmployeeId,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate,
            Image = task.Image,
        }).ToList();

        return Ok(response);
    }
}