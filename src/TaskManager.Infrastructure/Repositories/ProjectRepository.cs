﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Repositories;

internal class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> FindAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate)
            .Include(t => t.Manager)
            .ToListAsync(cancellationToken);
    }
}
