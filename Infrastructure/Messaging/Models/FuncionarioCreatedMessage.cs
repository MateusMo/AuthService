namespace Infrastructure.Messaging.Models;

public class FuncionarioCreatedMessage : BaseMessage
{
    public FuncionarioCreatedMessage()
    {
        EventType = "FuncionarioCreated";
    }

    public string FuncionarioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DateTime DataCriacao { get; set; }
}