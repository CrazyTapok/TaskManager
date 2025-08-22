namespace TaskManager.Core.Models.Email;

public class SmtpConnectionOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
}
