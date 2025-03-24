using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(Employee employee);
}
