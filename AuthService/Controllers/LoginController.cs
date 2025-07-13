using AuthService.DTO;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public LoginController(
        IFuncionarioRepository funcionarioRepository,
        IJwtService jwtService,
        IPasswordService passwordService)
    {
        _funcionarioRepository = funcionarioRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    [HttpGet, Route("Healthy")]
    public async Task<IActionResult> Healthy()
    {
        return Ok(new { status = "API funcionando!", timestamp = DateTime.UtcNow });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar funcionário pelo email
            var funcionario = await _funcionarioRepository.GetByEmailAsync(loginDto.Email);
            
            if (funcionario == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            // Verificar senha
            if (!_passwordService.VerifyPassword(loginDto.Senha, funcionario.Senha))
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            // Gerar token JWT
            var token = _jwtService.GenerateToken(funcionario);

            var response = new TokenResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Usuario = new GerenteResponseDto
                {
                    Id = funcionario.Id,
                    Nome = funcionario.Nome,
                    Email = funcionario.Email,
                    Tipo = funcionario.Tipo,
                    DataCriacao = funcionario.DataCriacao,
                    Nivel = funcionario is Domain.Gerente gerente ? gerente.Nivel : 1
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}