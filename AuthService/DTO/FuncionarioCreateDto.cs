using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO;

public class FuncionarioCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo é obrigatório")]
    public string Tipo { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Nível deve estar entre 1 e 5")]
    public int Nivel { get; set; } = 1;
}