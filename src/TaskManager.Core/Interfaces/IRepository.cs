namespace TaskManager.Core.Interfaces;

public interface IRepository<TEntity, TModel> where TEntity : class where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync();
    Task<TModel> GetByIdAsync(Guid id);
    Task AddAsync(TModel model);
    Task UpdateAsync(TModel model);
    Task DeleteAsync(Guid id);
}