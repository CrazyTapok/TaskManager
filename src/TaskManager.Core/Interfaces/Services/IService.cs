namespace TaskManager.Core.Interfaces.Services;

public interface IService<TModel>
{
    Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TModel>> ListAllAsync(CancellationToken cancellationToken);
    Task<TModel> AddAsync(TModel entity, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(TModel entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
