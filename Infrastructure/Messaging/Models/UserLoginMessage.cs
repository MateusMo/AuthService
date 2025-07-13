namespace Infrastructure.Messaging.Models;

public class UserLoginMessage : BaseMessage
{
    public UserLoginMessage()
    {
        EventType = "UserLogin";
    }

    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
}