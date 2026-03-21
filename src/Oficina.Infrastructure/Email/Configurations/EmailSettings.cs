namespace Oficina.Infrastructure.Email.Configurations;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 2525;
    public bool EnableSsl { get; set; }
    public string From { get; set; } = "oficina@localhost";
    public string BaseUrl { get; set; } = "http://localhost:8080";
}
