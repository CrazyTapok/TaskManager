using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

public class Service<TModel>(IRepository<TModel> repository) : IService<TModel> where TModel : class
{
    public async Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"Entity with id {id} not found", "");

        return entity;
    }

    public Task<List<TModel>> ListAllAsync(CancellationToken cancellationToken)
    {
        return repository.GetAllAsync(cancellationToken);
    }

    public Task<TModel> AddAsync(TModel entity, CancellationToken cancellationToken)
    {
        return repository.AddAsync(entity, cancellationToken);
    }

    public Task<bool> UpdateAsync(TModel entity, CancellationToken cancellationToken)
    {
        return repository.UpdateAsync(entity, cancellationToken);
    }
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return repository.DeleteAsync(id, cancellationToken);
    }
}
