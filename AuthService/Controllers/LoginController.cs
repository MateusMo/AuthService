using AuthService.DTO;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IMessageProducer _messageProducer;
    private readonly RabbitMQSettings _rabbitSettings;

    public LoginController(
        IFuncionarioRepository funcionarioRepository,
        IJwtService jwtService,
        IPasswordService passwordService,
        IMessageProducer messageProducer,
        IOptions<RabbitMQSettings> rabbitSettings)
    {
        _funcionarioRepository = funcionarioRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _messageProducer = messageProducer;
        _rabbitSettings = rabbitSettings.Value;
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

            // Publicar mensagem de login no RabbitMQ
            var message = new UserLoginMessage
            {
                UserId = funcionario.Id,
                Email = funcionario.Email,
                Nome = funcionario.Nome,
                Tipo = funcionario.Tipo,
                LoginTime = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP não disponível"
            };
            
            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.UserLogin);

            // Mantendo exatamente a mesma estrutura de retorno
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