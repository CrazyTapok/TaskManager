using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Interfaces.Services.Security;
using TaskManager.Core.Models;
using TaskManager.Core.Services.Base;

namespace TaskManager.Core.Services.ProjectManagement;

internal class EmployeeService : Service<Employee>, IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;

    public EmployeeService(IEmployeeRepository employeeRepository, IPasswordHasher passwordHasher) : base(employeeRepository)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Employee?> RegisterAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(employee.Email, nameof(employee.Email));

        var existingEmployee = await _employeeRepository.GetByEmailAsync(employee.Email, cancellationToken);
        if (existingEmployee != null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        employee.Password = _passwordHasher.HashPassword(employee.Password);

        return await _employeeRepository.AddAsync(employee, cancellationToken);
    }

    public Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.FindAsync(employee => employee.Projects.Any(project => project.Id == projectId), cancellationToken);
    }

    public Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.FindAsync(employee => employee.CompanyId == companyId, cancellationToken);
    }

    public Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        return _employeeRepository.GetByEmailAsync(email, cancellationToken);
    }

    public Task<List<Employee>> GetEmployeesWithDailyNewsletterEnabledAsync(CancellationToken cancellationToken = default)
    {
        return _employeeRepository.GetNewsletterEnabledEmployeesAsync(cancellationToken);
    }
}
