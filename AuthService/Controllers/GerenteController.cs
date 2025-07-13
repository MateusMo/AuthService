using AuthService.DTO;
using Domain;
using Infrastructure.Configuration;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GerenteController : ControllerBase
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMessageProducer _messageProducer; 
    private readonly RabbitMQSettings _rabbitSettings; 

    public GerenteController(
        IFuncionarioRepository funcionarioRepository,
        IPasswordService passwordService,
        IMessageProducer messageProducer, 
        IOptions<RabbitMQSettings> rabbitSettings) 
    {
        _funcionarioRepository = funcionarioRepository;
        _passwordService = passwordService;
        _messageProducer = messageProducer; 
        _rabbitSettings = rabbitSettings.Value; 
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GerenteResponseDto>>> GetAll()
    {
        try
        {
            var gerentes = await _funcionarioRepository.GetByTypeAsync("Gerente");
            
            var response = gerentes.Cast<Gerente>().Select(g => new GerenteResponseDto
            {
                Id = g.Id,
                Nome = g.Nome,
                Email = g.Email,
                Nivel = g.Nivel,
                Tipo = g.Tipo,
                DataCriacao = g.DataCriacao
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GerenteResponseDto>> GetById(string id)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            
            if (funcionario == null || funcionario.Tipo != "Gerente")
            {
                return NotFound(new { message = "Gerente não encontrado" });
            }

            var gerente = (Gerente)funcionario;
            var response = new GerenteResponseDto
            {
                Id = gerente.Id,
                Nome = gerente.Nome,
                Email = gerente.Email,
                Nivel = gerente.Nivel,
                Tipo = gerente.Tipo,
                DataCriacao = gerente.DataCriacao
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<GerenteResponseDto>> Create([FromBody] GerenteCreateDto gerenteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _funcionarioRepository.ExistsByEmailAsync(gerenteDto.Email))
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            var gerente = new Gerente
            {
                Nome = gerenteDto.Nome,
                Email = gerenteDto.Email,
                Senha = _passwordService.HashPassword(gerenteDto.Senha),
                Nivel = gerenteDto.Nivel
            };

            var gerenteCriado = await _funcionarioRepository.CreateAsync(gerente);
            var gerenteResponse = (Gerente)gerenteCriado;

            // 🆕 NOVO: Publicar mensagem no RabbitMQ
            var message = new GerenteCreatedMessage
            {
                GerenteId = gerenteResponse.Id,
                Nome = gerenteResponse.Nome,
                Email = gerenteResponse.Email,
                Nivel = gerenteResponse.Nivel,
                DataCriacao = gerenteResponse.DataCriacao
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.GerenteCreated);

            var response = new GerenteResponseDto
            {
                Id = gerenteResponse.Id,
                Nome = gerenteResponse.Nome,
                Email = gerenteResponse.Email,
                Nivel = gerenteResponse.Nivel,
                Tipo = gerenteResponse.Tipo,
                DataCriacao = gerenteResponse.DataCriacao
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<GerenteResponseDto>> Update(string id, [FromBody] GerenteUpdateDto gerenteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcionarioExistente = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionarioExistente == null || funcionarioExistente.Tipo != "Gerente")
            {
                return NotFound(new { message = "Gerente não encontrado" });
            }

            var gerenteExistente = (Gerente)funcionarioExistente;

            var funcionarioComEmail = await _funcionarioRepository.GetByEmailAsync(gerenteDto.Email);
            if (funcionarioComEmail != null && funcionarioComEmail.Id != id)
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            gerenteExistente.Nome = gerenteDto.Nome;
            gerenteExistente.Email = gerenteDto.Email;
            gerenteExistente.Nivel = gerenteDto.Nivel;

            var sucesso = await _funcionarioRepository.UpdateAsync(id, gerenteExistente);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao atualizar gerente" });
            }

            // 🆕 NOVO: Publicar mensagem no RabbitMQ
            var message = new GerenteUpdatedMessage
            {
                GerenteId = gerenteExistente.Id,
                Nome = gerenteExistente.Nome,
                Email = gerenteExistente.Email,
                Nivel = gerenteExistente.Nivel
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.GerenteUpdated);

            var response = new GerenteResponseDto
            {
                Id = gerenteExistente.Id,
                Nome = gerenteExistente.Nome,
                Email = gerenteExistente.Email,
                Nivel = gerenteExistente.Nivel,
                Tipo = gerenteExistente.Tipo,
                DataCriacao = gerenteExistente.DataCriacao
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionario == null || funcionario.Tipo != "Gerente")
            {
                return NotFound(new { message = "Gerente não encontrado" });
            }

            var gerente = (Gerente)funcionario;

            var sucesso = await _funcionarioRepository.DeleteAsync(id);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao deletar gerente" });
            }

            // 🆕 NOVO: Publicar mensagem no RabbitMQ
            var message = new GerenteDeletedMessage
            {
                GerenteId = gerente.Id,
                Nome = gerente.Nome,
                Email = gerente.Email
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.GerenteDeleted);

            return Ok(new { message = "Gerente deletado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<ActionResult<GerenteResponseDto>> GetByEmail(string email)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByEmailAsync(email);
            
            if (funcionario == null || funcionario.Tipo != "Gerente")
            {
                return NotFound(new { message = "Gerente não encontrado" });
            }

            var gerente = (Gerente)funcionario;
            var response = new GerenteResponseDto
            {
                Id = gerente.Id,
                Nome = gerente.Nome,
                Email = gerente.Email,
                Nivel = gerente.Nivel,
                Tipo = gerente.Tipo,
                DataCriacao = gerente.DataCriacao
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}