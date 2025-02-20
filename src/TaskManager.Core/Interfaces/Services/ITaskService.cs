﻿using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Services;

public interface ITaskService : IService<Task>
{
    Task<List<Task>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);

    Task<List<Task>> GetTasksByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken);
}
