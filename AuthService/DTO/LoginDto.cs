using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [MaxLength(100, ErrorMessage = "O email não pode exceder 100 caracteres")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "A senha deve conter pelo menos: 1 letra maiúscula, 1 minúscula, 1 número e 1 caractere especial"
    )]
    public string Senha { get; set; }
}