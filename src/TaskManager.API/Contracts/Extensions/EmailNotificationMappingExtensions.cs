using TaskManager.API.Contracts.Requests;
using TaskManager.Core.Models;

namespace TaskManager.API.Contracts.Extensions;

public static class EmailNotificationMappingExtensions
{
    public static EmailNotification ToEmailNotification(this EmailNotificationRequest request)
    {
        return new EmailNotification
        {
            Subject = request.Subject,
            RecipientName = request.RecipientName,
            RecipientEmail = request.RecipientEmail
        };
    }
}