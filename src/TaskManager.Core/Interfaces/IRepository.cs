namespace TaskManager.Core.Interfaces;

public interface IRepository<TModel> where TModel : class
{
    Task<List<TModel>> GetAllAsync();
    Task<TModel> GetByIdAsync(Guid id);
    Task AddAsync(TModel model);
    Task UpdateAsync(TModel model);
    Task DeleteAsync(Guid id);
}