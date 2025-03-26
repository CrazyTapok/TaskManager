namespace TaskManager.API.Contracts.Requests;

public record EmailNotificationRequest(
    List<string> EmailList,
    string Subject,
    string Body,
    string CronExpression,
    string CreatedByName,
    string CreatedByEmail
);