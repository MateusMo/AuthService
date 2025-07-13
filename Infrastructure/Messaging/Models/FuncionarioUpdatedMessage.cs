namespace Infrastructure.Messaging.Models;

public class FuncionarioUpdatedMessage : BaseMessage
{
    public FuncionarioUpdatedMessage()
    {
        EventType = "FuncionarioUpdated";
    }

    public string FuncionarioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}