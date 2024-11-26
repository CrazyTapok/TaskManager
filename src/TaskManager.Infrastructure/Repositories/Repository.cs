using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Repositories;

internal class Repository<TModel> : IRepository<TModel> where TModel : class
{
    protected readonly DbContext _context;
    private readonly DbSet<TModel> _dbSet;
    public Repository(DbContext context) 
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TModel>();
    }

    public Task<List<TModel>> GetAllAsync()
    {
        return _dbSet.ToListAsync();
    }

    public Task<TModel> GetByIdAsync(Guid id)
    {
        return _dbSet.FindAsync(id).AsTask();
    }

    public async Task AddAsync(TModel model)
    {
        await _dbSet.AddAsync(model);
      
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(TModel model)
    {
        _context.Entry(model).State = EntityState.Modified;

        return _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            typeof(TModel).GetProperty("IsDeleted").SetValue(entity, true);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
