using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Services;

public class TaskService(IRepository<Task> taskRepository) : Service<Task>(taskRepository), ITaskService
{
    private readonly IRepository<Task> _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));


    public Task<List<Task>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return _taskRepository.FindAsync(t => t.ProjectId == projectId, cancellationToken);
    }

    public Task<List<Task>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        return _taskRepository.FindAsync(t => t.AssignedEmployeeId == employeeId || t.CreateEmployeeId == employeeId, cancellationToken); 
    }
}
