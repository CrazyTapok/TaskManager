using TaskManager.Core.Enums;

namespace TaskManager.API.Contracts.Responses;

public record EmployeeResponse(
    Guid Id,
    string Name,
    string Email,
    Guid CompanyId,
    Role Role
);