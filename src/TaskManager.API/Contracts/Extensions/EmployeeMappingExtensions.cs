using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Models;

namespace TaskManager.API.Contracts.Extensions;

public static class EmployeeMappingExtensions
{
    public static EmployeeResponse MapToEmployeeResponse(this Employee employee)
    {
        return new EmployeeResponse(employee.Id, employee.Name, employee.Email, employee.CompanyId, employee.Role);
    }

    public static Employee ToEmployee(this EmployeeRequest request, Guid? id = null)
    {
        return new Employee
        {
            Id = id ?? Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            CompanyId = request.CompanyId,
            Role = request.Role
        };
    }
}
