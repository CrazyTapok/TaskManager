using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Mappings;

namespace TaskManager.Infrastructure.Repositories;

internal class Repository<TEntity, TModel> : IRepository<TEntity, TModel> where TEntity : class where TModel : class
{
    protected readonly DBContext _context;
    private readonly DbSet<TEntity> _dbSet;
    public Repository(DBContext context) 
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TModel>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();

        return entities.Map<TEntity, TModel>();
    }

    public async Task<TModel> GetByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity != null)
        {
            return entity.Map<TEntity, TModel>();
        }

        return null;
    }

    public async Task AddAsync(TModel model)
    {
        var entity = model.Map<TModel, TEntity>();
        await _dbSet.AddAsync(entity);
      
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(TModel model)
    {
        var entity = model.Map<TModel, TEntity>();
        _context.Entry(entity).State = EntityState.Modified;

        return _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            
            await _context.SaveChangesAsync();
        }
    }
}
