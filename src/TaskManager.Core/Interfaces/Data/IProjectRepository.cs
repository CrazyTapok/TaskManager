using System.Linq.Expressions;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Data;

public interface IProjectRepository : IRepository<Project> 
{
    Task<IEnumerable<Project>> FindAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken);
}