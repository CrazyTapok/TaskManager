using TaskManager.Core.Interfaces.Services.Authentication;
using TaskManager.Core.Interfaces.Services.ProjectManagement;
using TaskManager.Core.Interfaces.Services.Security;

namespace TaskManager.Core.Services.Authentication;

internal class AuthService(IEmployeeService employeeService, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher) : IAuthService
{

    public async Task<string> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var employee = await employeeService.GetEmployeeByEmailAsync(email, cancellationToken);
        if (employee == null || !passwordHasher.VerifyPassword(password, employee.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return jwtTokenService.GenerateToken(employee);
    }
}
