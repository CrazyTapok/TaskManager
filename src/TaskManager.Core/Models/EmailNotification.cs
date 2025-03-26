namespace TaskManager.Core.Models;

public class EmailNotification
{
    public string JobId { get; set; }
    public List<string> EmailList { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string CronExpression { get; set; }
    public string CreatedByName { get; set; }
    public string CreatedByEmail { get; set; }
}
