namespace TaskManager.API.Contracts.Responses;

public record ProjectResponse(
    Guid Id,
    string Title,
    Guid ManagerId,
    Guid CompanyId
);