using TaskManager.Core.Enums;

namespace TaskManager.API.Contracts.Response;

public class EmployeeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Guid CompanyId { get; set; }
    public Role Role { get; set; }
}
