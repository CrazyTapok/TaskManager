using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Data;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
