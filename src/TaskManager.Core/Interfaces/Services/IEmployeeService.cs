using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IEmployeeService : IService<Employee>
{
    Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);

    Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);

    Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken cancellationToken);

    Task<Employee?> RegisterAsync(Employee employee, CancellationToken cancellationToken);

    Task<List<Employee>> GetEmployeesWithDailyNewsletterEnabledAsync(CancellationToken cancellationToken = default);
}
