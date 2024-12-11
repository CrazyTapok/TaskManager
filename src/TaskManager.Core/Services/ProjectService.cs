using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public async Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _projectRepository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
            throw new ValidationException($"Entity with id {id} not found", "");

        return entity;
    }

    public async Task<List<Project>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await _projectRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Project> AddAsync(Project entity, CancellationToken cancellationToken)
    {
        return await _projectRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Project entity, CancellationToken cancellationToken)
    {
        return await _projectRepository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Project entity, CancellationToken cancellationToken)
    {
        await _projectRepository.DeleteAsync(entity.Id, cancellationToken);
    }

    public async Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid emploeeId, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.FindAsync(e => e.Employees.Any(p => p.Id == emploeeId), cancellationToken);

        return projects.ToList();
    }
}
