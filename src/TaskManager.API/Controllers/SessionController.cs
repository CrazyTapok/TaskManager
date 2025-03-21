using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest("Request cannot be null.");
        }

        try
        {
            var token = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
            return Ok(new LoginResponse(token));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}