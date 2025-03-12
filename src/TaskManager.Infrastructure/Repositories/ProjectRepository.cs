using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;

namespace TaskManager.Infrastructure.Repositories;

internal class ProjectRepository(DBContext context) : Repository<Project>(context)
{
    public override Task<List<Project>> FindAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate)
            .Include(project => project.Manager)
            .ToListAsync(cancellationToken);
    }
}
