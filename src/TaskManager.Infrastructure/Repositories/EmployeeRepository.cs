using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;

namespace TaskManager.Infrastructure.Repositories;

internal class EmployeeRepository(DBContext context) : Repository<Employee>(context), IEmployeeRepository
{
    public override Task<List<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().Where(predicate)
            .Include(employee => employee.Company)
            .ToListAsync(cancellationToken);
    }

    public Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().FirstOrDefaultAsync(employee => employee.Email == email, cancellationToken);
    }

    public Task<List<Employee>> GetNewsletterEnabledEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking()
         .Where(employee => employee.IsDailyNewsletterEnabled)
         .Include(employee => employee.ManagerProjects)
             .ThenInclude(project => project.Tasks)
         .ToListAsync(cancellationToken);
    }
}