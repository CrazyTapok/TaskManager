using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Utilities;

namespace TaskManager.Core.Services;

internal class AuthService(IEmployeeService employeeService, IJwtTokenService jwtTokenService) : IAuthService
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<string> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService.GetEmployeeByEmailAsync(email, cancellationToken);
        if (employee == null || !PasswordHelper.VerifyPassword(password, employee.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return _jwtTokenService.GenerateToken(employee);
    }
}
