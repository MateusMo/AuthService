namespace Infrastructure.Messaging.Models;

public class GerenteUpdatedMessage : BaseMessage
{
    public GerenteUpdatedMessage()
    {
        EventType = "GerenteUpdated";
    }

    public string GerenteId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}