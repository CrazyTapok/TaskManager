using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.EF;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Repositories;

internal class Repository<TModel>(DBContext context) : IRepository<TModel> where TModel : BaseEntity
{
    protected readonly DbSet<TModel> _dbSet = context.Set<TModel>();


    public Task<List<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.ToListAsync(cancellationToken);
    }
     
    public ValueTask<TModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<TModel> AddAsync(TModel model, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(model, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return model;
    }

    public async Task<bool> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        context.Entry(model).State = EntityState.Modified;

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken); 
        
        if (entity != null) 
        { 
            entity.IsDeleted = true;

            context.Entry(entity).State = EntityState.Modified; 
            await context.SaveChangesAsync(cancellationToken); 
        }
    }

    public Task<List<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}
