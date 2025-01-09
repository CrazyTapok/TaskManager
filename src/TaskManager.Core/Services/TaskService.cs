using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Services;

internal class TaskService : Service<Task>, ITaskService
{
    private readonly IRepository<Task> _taskRepository;

    public TaskService(IRepository<Task> taskRepository) : base(taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    public Task<List<Task>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return _taskRepository.FindAsync(task => task.ProjectId == projectId, cancellationToken);
    }

    public Task<List<Task>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return _taskRepository.FindAsync(task => task.AssignedEmployeeId == employeeId || task.CreateEmployeeId == employeeId, cancellationToken); 
    }
}
