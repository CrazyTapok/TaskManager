namespace TaskManager.API.Contracts.Responses;
using TaskStatus = Core.Enums.TaskStatus;

public record TaskResponse(
    Guid Id,
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