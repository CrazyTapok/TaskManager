using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services.Authentication;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController(IAuthService authService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
            return Ok(new LoginResponse(token));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}