using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Infrastructure.Repositories;

internal class TaskRepository : Repository<Task>
{
    public TaskRepository(DbContext context) : base(context)
    {
    }


    public async Task<IEnumerable<Task>> FindAsync(Expression<Func<Task, bool>> predicate, CancellationToken? cancellationToken)
    {
        return await _dbSet.Where(predicate)
            .Include(t => t.AssinedEmployee)
            .Include(t => t.CreateEmployee)
            .ToListAsync(cancellationToken ?? CancellationToken.None);
    }
}
