using System.Linq.Expressions;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Data;

public interface ITaskRepository : IRepository<Task>
{
    Task<IEnumerable<Task>> FindAsync(Expression<Func<Task, bool>> predicate, CancellationToken cancellationToken);
}