namespace TaskManager.Core.Interfaces.Services;

public interface IService<T>
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<T>> ListAllAsync(CancellationToken cancellationToken);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(T entity, CancellationToken cancellationToken);
}
