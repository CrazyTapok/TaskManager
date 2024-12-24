using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

internal interface IProjectService : IService<Project>
{
    Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid emploeeId, CancellationToken? cancellationToken);
}
