
namespace TaskManager.Core.Interfaces
{
    public interface IRepository<TEntity, TModels> where TEntity : class where TModels : class
    {
        Task<IEnumerable<TModels>> GetAllAsync();
        Task<TModels> GetByIdAsync(int id);
        Task AddAsync(TModels dto);
        Task UpdateAsync(TModels dto);
        Task DeleteAsync(int id);
    }
}
