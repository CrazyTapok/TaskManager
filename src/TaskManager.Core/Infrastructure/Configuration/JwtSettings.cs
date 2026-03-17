namespace TaskManager.Core.Infrastructure.Configuration;

public class JwtSettings
{
    public static readonly string SectionName = "JWT";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
