using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

public class Service<TModel> : IService<TModel> where TModel : class
{
    private readonly IRepository<TModel> _repository;

    public Service(IRepository<TModel> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<TModel> GetByIdAsync(Guid id, CancellationToken? cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"Entity with id {id} not found", "");

        return entity;
    }

    public Task<List<TModel>> ListAllAsync(CancellationToken? cancellationToken)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<TModel> AddAsync(TModel entity, CancellationToken? cancellationToken)
    {
        return _repository.AddAsync(entity, cancellationToken);
    }

    public Task<bool> UpdateAsync(TModel entity, CancellationToken? cancellationToken)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
    public Task DeleteAsync(Guid id, CancellationToken? cancellationToken)
    {
        return _repository.DeleteAsync(id, cancellationToken);
    }
}
