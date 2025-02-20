using TaskManager.Core.Enums;

namespace TaskManager.API.Contracts.Request;

public class EmployeeRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid CompanyId { get; set; }
    public Role Role { get; set; }
}
