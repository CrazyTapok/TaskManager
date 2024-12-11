using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Repositories;

internal class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate)
            .Include(t => t.Company)
            .ToListAsync(cancellationToken);
    }
}
