namespace Infrastructure.Messaging.Models;

public class FuncionarioDeletedMessage : BaseMessage
{
    public FuncionarioDeletedMessage()
    {
        EventType = "FuncionarioDeleted";
    }

    public string FuncionarioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataDelecao { get; set; } = DateTime.UtcNow;
}