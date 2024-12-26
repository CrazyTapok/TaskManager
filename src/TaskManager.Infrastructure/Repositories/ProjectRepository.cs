using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;

namespace TaskManager.Infrastructure.Repositories;

internal class ProjectRepository(DBContext context) : Repository<Project>(context)
{
    public new Task<List<Project>> FindAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken)
    {
        return _dbSet.Where(predicate)
            .Include(t => t.Manager)
            .ToListAsync(cancellationToken);
    }
}
