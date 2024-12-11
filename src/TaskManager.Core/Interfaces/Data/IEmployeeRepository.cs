using System.Linq.Expressions;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Data;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate, CancellationToken cancellationToken);
}