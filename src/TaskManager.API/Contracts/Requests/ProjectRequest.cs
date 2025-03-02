namespace TaskManager.API.Contracts.Requests;

public record ProjectRequest(
    string Title,
    Guid ManagerId,
    Guid CompanyId
);