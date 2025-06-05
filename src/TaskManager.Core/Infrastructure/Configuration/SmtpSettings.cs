namespace TaskManager.Core.Infrastructure.Configuration;

public class SmtpSettings
{
    public static readonly string SectionName = "SMTP";
    public string User { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}
