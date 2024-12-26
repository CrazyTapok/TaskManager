using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Infrastructure.EF;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Infrastructure.Repositories;

internal class TaskRepository(DBContext context) : Repository<Task>(context)
{
    public new Task<List<Task>> FindAsync(Expression<Func<Task, bool>> predicate, CancellationToken cancellationToken)
    {
        return _dbSet.Where(predicate)
            .Include(t => t.AssignedEmployee)
            .Include(t => t.CreateEmployee)
            .ToListAsync(cancellationToken);
    }
}
