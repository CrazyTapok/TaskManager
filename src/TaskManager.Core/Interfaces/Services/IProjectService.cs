using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IProjectService : IService<Project>
{
    Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken);
}
