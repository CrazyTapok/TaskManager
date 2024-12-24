using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Services;

public class TaskService : Service<Task>, ITaskService
{
    private readonly IRepository<Task> _taskRepository;

    public TaskService(IRepository<Task> taskRepository) : base(taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
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
