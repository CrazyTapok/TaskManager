using Hangfire;

namespace TaskManager.Core.Infrastructure.Configuration;

public class NotificationSettings
{
    public static readonly string SectionName = "NOTIFICATION";
    public required string Id { get; set; }
    public TimeSpan Time { get; set; }
    public string CronExpression => Cron.Daily(Time.Hours, Time.Minutes);
}