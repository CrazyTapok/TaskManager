using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Interfaces.Data;

namespace TaskManager.Infrastructure.Repositories;

internal class Repository<TModel> : IRepository<TModel> where TModel : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TModel> _dbSet;

    public Repository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TModel>();
    }

    public Task<List<TModel>> GetAllAsync(CancellationToken? cancellationToken)
    {
        return _dbSet.ToListAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task<TModel> GetByIdAsync(Guid id, CancellationToken? cancellationToken)
    {
        return _dbSet.FindAsync(id, cancellationToken ?? CancellationToken.None).AsTask();
    }

    public async Task<TModel> AddAsync(TModel model, CancellationToken? cancellationToken)
    {
        await _dbSet.AddAsync(model);

        await _context.SaveChangesAsync(cancellationToken ?? CancellationToken.None);

        return model;
    }

    public async Task<bool> UpdateAsync(TModel model, CancellationToken? cancellationToken)
    {
        _context.Entry(model).State = EntityState.Modified;

        var result = await _context.SaveChangesAsync(cancellationToken ?? CancellationToken.None);

        return result > 0;
    }

    public async Task DeleteAsync(Guid id, CancellationToken? cancellationToken)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken ?? CancellationToken.None);
        if (entity != null)
        {
            typeof(TModel).GetProperty("IsDeleted")?.SetValue(entity, true);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken? cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken ?? CancellationToken.None);
    }
}
