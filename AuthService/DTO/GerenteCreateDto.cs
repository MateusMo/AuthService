using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO;

public class GerenteCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Nível deve estar entre 1 e 10")]
    public int Nivel { get; set; } = 1;
}

public class GerenteUpdateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Nível deve estar entre 1 e 10")]
    public int Nivel { get; set; } = 1;
}

public class GerenteResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}