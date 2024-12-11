using System.Linq.Expressions;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Data;

public interface ICompanyRepository : IRepository<Company>
{
    Task<IEnumerable<Company>> FindAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken);
}