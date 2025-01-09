using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;

namespace TaskManager.Infrastructure.Repositories;

internal class EmployeeRepository(DBContext context) : Repository<Employee>(context)
{
    public new Task<List<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate)
            .Include(employee => employee.Company)
            .ToListAsync(cancellationToken);
    }
}
