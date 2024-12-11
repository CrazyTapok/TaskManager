using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Repositories;

internal class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Company>> FindAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}
