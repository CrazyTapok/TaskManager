using TaskManager.Core.Interfaces.Services.Core;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services.ProjectManagement;

public interface IProjectService : IService<Project>
{
    Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken);
}
