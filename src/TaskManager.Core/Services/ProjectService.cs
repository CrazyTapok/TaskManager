using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class ProjectService(IRepository<Project> projectRepository) : Service<Project>(projectRepository), IProjectService
{
    private readonly IRepository<Project> _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));


    public Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        return _projectRepository.FindAsync(e => e.Employees.Any(p => p.Id == employeeId), cancellationToken);
    }
}
