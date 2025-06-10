using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Models;
using TaskManager.Core.Services.Base;

namespace TaskManager.Core.Services.ProjectManagement;

internal class ProjectService : Service<Project>, IProjectService
{
    private readonly IRepository<Project> _projectRepository;

    public ProjectService(IRepository<Project> projectRepository) : base(projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return _projectRepository.FindAsync(project => project.Employees.Any(employee => employee.Id == employeeId), cancellationToken);
    }
}
