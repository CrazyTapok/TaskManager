using TaskManager.Core.Enums;

namespace TaskManager.API.Contracts.Requests;

public record EmployeeRequest(
    string Name,
    string Email,
    string Password,
    Guid CompanyId,
    Role Role
);