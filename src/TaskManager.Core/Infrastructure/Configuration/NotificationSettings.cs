namespace TaskManager.Core.Infrastructure.Configuration;

public class NotificationSettings
{
    public static readonly string SectionName = "NOTIFICATION";
    public required string Id { get; set; }
    public string CronExpression { get; set; }
}