namespace TaskManager.API.Contracts.Requests;

public record EmailNotificationRequest(
    string Subject,        
    string RecipientName,  
    string RecipientEmail
);