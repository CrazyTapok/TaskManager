using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.API.Contracts.Extensions;

public static class TaskMappingExtensions
{
    public static TaskResponse MapToTaskResponse(this Task task)
    {
        return new TaskResponse(task.Id, task.Title, task.Description, task.Status, task.ProjectId, task.CreateEmployeeId, task.AssignedEmployeeId, task.CreatedDate, task.UpdatedDate, task.Image);
    }

    public static Task ToTask(this TaskRequest request, Guid? id = null)
    {
        return new Task
        {
            Id = id ?? Guid.NewGuid(),
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
    }
}
