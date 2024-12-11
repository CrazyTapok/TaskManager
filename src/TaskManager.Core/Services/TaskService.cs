using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    public async Task<Task> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _taskRepository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
            throw new ValidationException($"Entity with id {id} not found", "");

        return entity;
    }

    public async Task<List<Task>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await _taskRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Task> AddAsync(Task entity, CancellationToken cancellationToken)
    {
        return await _taskRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Task entity, CancellationToken cancellationToken)
    {
        return await _taskRepository.UpdateAsync(entity, cancellationToken);
    }

    public async System.Threading.Tasks.Task DeleteAsync(Task entity, CancellationToken cancellationToken)
    {
        await _taskRepository.DeleteAsync(entity.Id, cancellationToken);
    }

    public async Task<List<Task>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.FindAsync(t => t.ProjectId == projectId, cancellationToken);

        return tasks.ToList();
    }

    public async Task<List<Task>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.FindAsync(t => t.AssinedEmployeeId == employeeId || t.CreateEmployeeId == employeeId, cancellationToken); 
        
        return tasks.ToList();
    }
}
