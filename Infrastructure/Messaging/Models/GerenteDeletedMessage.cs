namespace Infrastructure.Messaging.Models;

public class GerenteDeletedMessage : BaseMessage
{
    public GerenteDeletedMessage()
    {
        EventType = "GerenteDeleted";
    }

    public string GerenteId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataDelecao { get; set; } = DateTime.UtcNow;
}