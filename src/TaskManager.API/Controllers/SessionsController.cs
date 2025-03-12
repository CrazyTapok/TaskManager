using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Utilities;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController(IEmployeeService employeeService, IJwtTokenService jwtTokenService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var employee = await _employeeService.GetEmployeeByEmailAsync(request.Email, cancellationToken);
        if (employee == null || !PasswordHelper.VerifyPassword(request.Password, employee.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _jwtTokenService.GenerateToken(employee);
        return Ok(new LoginResponse(token));
    }
}