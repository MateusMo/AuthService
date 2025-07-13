namespace AuthService.DTO;

public class FuncionarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DateTime DataCriacao { get; set; }
}