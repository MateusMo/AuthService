namespace Infrastructure.Messaging.Models;

public class GerenteCreatedMessage : BaseMessage
{
    public GerenteCreatedMessage()
    {
        EventType = "GerenteCreated";
    }

    public string GerenteId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DateTime DataCriacao { get; set; }
}