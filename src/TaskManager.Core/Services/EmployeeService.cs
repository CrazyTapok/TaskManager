using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class EmployeeService : Service<Employee>, IEmployeeService 
{
    private readonly IRepository<Employee> _employeeRepository; 
    
    public EmployeeService(IRepository<Employee> employeeRepository) : base(employeeRepository) 
    { 
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository)); 
    }


    public async Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken? cancellationToken) 
    { 
        var employees = await _employeeRepository.FindAsync(t => t.Projects.Any(t => t.Id == projectId), cancellationToken); 
        
        return employees.ToList(); 
    }

    public async Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken? cancellationToken)
    {
        var employees = await _employeeRepository.FindAsync(t => t.CompanyId == companyId, cancellationToken);

        return employees.ToList();
    }
}
