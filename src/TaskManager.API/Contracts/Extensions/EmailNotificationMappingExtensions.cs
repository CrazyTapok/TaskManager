using TaskManager.API.Contracts.Requests;
using TaskManager.Core.Models;

namespace TaskManager.API.Contracts.Extensions;

public static class EmailNotificationMappingExtensions
{
    public static EmailNotification ToEmailNotification(this EmailNotificationRequest request)
    {
        return new EmailNotification
        {
            JobId = Guid.NewGuid().ToString(),
            EmailList = request.EmailList,
            Subject = request.Subject,
            Body = request.Body,
            CronExpression = request.CronExpression,
            CreatedByName = request.CreatedByName,
            CreatedByEmail = request.CreatedByEmail
        };
    }
}