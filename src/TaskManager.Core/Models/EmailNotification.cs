namespace TaskManager.Core.Models;

public class EmailNotification
{
    public string Subject { get; set; }
    public string RecipientName { get; set; }
    public string RecipientEmail { get; set; }
    public string Body {  get; set; }
}
