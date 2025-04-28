using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Extensions;
using TaskManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController(IProjectNotificationService projectNotificationService) : ControllerBase
{
    private readonly IProjectNotificationService _projectNotificationService = projectNotificationService;

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost("enable")]
    public IActionResult EnableNotifications(Guid projectId, [FromBody] EmailNotificationRequest request)
    {
        var emailNotification = request.ToEmailNotification();

        _projectNotificationService.EnableNotifications(projectId, emailNotification);
       
        return Ok();
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost("disable")]
    public IActionResult DisableNotifications(Guid projectId)
    {
        _projectNotificationService.DisableNotifications(projectId);
        
        return Ok();
    }
}