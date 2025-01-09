using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

internal class EmployeeService : Service<Employee>, IEmployeeService 
{
    private readonly IRepository<Employee> _employeeRepository;

    public EmployeeService(IRepository<Employee> employeeRepository) : base(employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) 
    {
        return _employeeRepository.FindAsync(employee => employee.Projects.Any(project => project.Id == projectId), cancellationToken); 
        
    }

    public Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.FindAsync(employee => employee.CompanyId == companyId, cancellationToken);
    }
}
