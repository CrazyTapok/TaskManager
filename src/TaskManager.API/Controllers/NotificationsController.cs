using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Extensions;
using TaskManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController(INotificationSchedulerService notificationSchedulerService) : ControllerBase
{
    private readonly INotificationSchedulerService _notificationSchedulerService = notificationSchedulerService;

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost("schedule")]
    public IActionResult ScheduleEmailJob([FromBody] EmailNotificationRequest request)
    {
        var emailNotification = request.ToEmailNotification();

        _notificationSchedulerService.ScheduleEmailJob(emailNotification);

        return Ok();
    }
}