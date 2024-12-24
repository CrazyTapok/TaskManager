using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class ProjectService : Service<Project>, IProjectService
{
    private readonly IRepository<Project> _projectRepository;

    public ProjectService(IRepository<Project> projectRepository) : base(projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public async Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid emploeeId, CancellationToken? cancellationToken)
    {
        var projects = await _projectRepository.FindAsync(e => e.Employees.Any(p => p.Id == emploeeId), cancellationToken);

        return projects.ToList();
    }
}
