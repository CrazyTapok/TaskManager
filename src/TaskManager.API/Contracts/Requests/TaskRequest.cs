namespace TaskManager.API.Contracts.Requests;
using TaskStatus = Core.Enums.TaskStatus;

public record TaskRequest(
    string Title,
    string Description,
    TaskStatus Status,
    Guid ProjectId,
    Guid CreateEmployeeId,
    Guid AssignedEmployeeId,
    DateTime CreatedDate,
    DateTime UpdatedDate,
    byte Image
);