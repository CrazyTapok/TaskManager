using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<Employee> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _employeeRepository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
            throw new ValidationException($"Entity with id {id} not found", "");

        return entity;
    }

    public async Task<List<Employee>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await _employeeRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken)
    {
        return await _employeeRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Employee entity, CancellationToken cancellationToken)
    {
        return await _employeeRepository.UpdateAsync(entity, cancellationToken);
    }
    public async Task DeleteAsync(Employee entity, CancellationToken cancellationToken)
    {
        await _employeeRepository.DeleteAsync(entity.Id, cancellationToken);
    }

    public async Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken) 
    { 
        var employees = await _employeeRepository.FindAsync(t => t.Projects.Any(t => t.Id == projectId), cancellationToken); 
        
        return employees.ToList(); 
    }

    public async Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.FindAsync(t => t.CompanyId == companyId, cancellationToken);

        return employees.ToList();
    }
}
