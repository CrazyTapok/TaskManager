using System.Linq.Expressions;

namespace TaskManager.Core.Interfaces.Data;

public interface IRepository<TModel> where TModel : class
{
    Task<List<TModel>> GetAllAsync(CancellationToken? cancellationToken);
    Task<TModel> GetByIdAsync(Guid id, CancellationToken? cancellationToken);
    Task<TModel> AddAsync(TModel model, CancellationToken? cancellationToken);
    Task<bool> UpdateAsync(TModel model, CancellationToken? cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken? cancellationToken);
    Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken? cancellationToken);
}