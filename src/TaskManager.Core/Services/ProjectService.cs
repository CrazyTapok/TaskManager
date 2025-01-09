using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

internal class ProjectService : Service<Project>, IProjectService
{
    private readonly IRepository<Project> _projectRepository;

    public ProjectService(IRepository<Project> projectRepository) : base(projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return _projectRepository.FindAsync(project => project.Employees.Any(employee => employee.Id == employeeId), cancellationToken);
    }
}
