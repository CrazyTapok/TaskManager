namespace TaskManager.Core.Infrastructure.Configuration;

public class NotificationSettings
{
    public static readonly string SectionName = "NOTIFICATION";
    public TimeSpan NotificationTime { get; set; }
}