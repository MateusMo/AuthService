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
public class FuncionarioController : ControllerBase
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMessageProducer _messageProducer;
    private readonly RabbitMQSettings _rabbitSettings;

    public FuncionarioController(
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
    public async Task<ActionResult<IEnumerable<FuncionarioResponseDto>>> GetAll()
    {
        try
        {
            var funcionarios = await _funcionarioRepository.GetAllAsync();
            
            var response = funcionarios.Select(f => new FuncionarioResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                Email = f.Email,
                Tipo = f.Tipo,
                Nivel = f.Nivel,
                DataCriacao = f.DataCriacao
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
    public async Task<ActionResult<FuncionarioResponseDto>> GetById(string id)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            
            if (funcionario == null)
            {
                return NotFound(new { message = "Funcionário não encontrado" });
            }

            var response = new FuncionarioResponseDto
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Tipo = funcionario.Tipo,
                Nivel = funcionario.Nivel,
                DataCriacao = funcionario.DataCriacao
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<FuncionarioResponseDto>> Create([FromBody] FuncionarioCreateDto funcionarioDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _funcionarioRepository.ExistsByEmailAsync(funcionarioDto.Email))
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            var funcionario = new Funcionario
            {
                Nome = funcionarioDto.Nome,
                Email = funcionarioDto.Email,
                Senha = _passwordService.HashPassword(funcionarioDto.Senha),
                Tipo = funcionarioDto.Tipo,
                Nivel = funcionarioDto.Nivel
            };

            var funcionarioCriado = await _funcionarioRepository.CreateAsync(funcionario);

            // Publicar mensagem no RabbitMQ
            var message = new FuncionarioCreatedMessage
            {
                FuncionarioId = funcionarioCriado.Id,
                Nome = funcionarioCriado.Nome,
                Email = funcionarioCriado.Email,
                Tipo = funcionarioCriado.Tipo,
                Nivel = funcionarioCriado.Nivel,
                DataCriacao = funcionarioCriado.DataCriacao
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.FuncionarioCreated);

            var response = new FuncionarioResponseDto
            {
                Id = funcionarioCriado.Id,
                Nome = funcionarioCriado.Nome,
                Email = funcionarioCriado.Email,
                Tipo = funcionarioCriado.Tipo,
                Nivel = funcionarioCriado.Nivel,
                DataCriacao = funcionarioCriado.DataCriacao
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
    public async Task<ActionResult<FuncionarioResponseDto>> Update(string id, [FromBody] FuncionarioUpdateDto funcionarioDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcionarioExistente = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionarioExistente == null)
            {
                return NotFound(new { message = "Funcionário não encontrado" });
            }

            var funcionarioComEmail = await _funcionarioRepository.GetByEmailAsync(funcionarioDto.Email);
            if (funcionarioComEmail != null && funcionarioComEmail.Id != id)
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            funcionarioExistente.Nome = funcionarioDto.Nome;
            funcionarioExistente.Email = funcionarioDto.Email;
            funcionarioExistente.Tipo = funcionarioDto.Tipo;
            funcionarioExistente.Nivel = funcionarioDto.Nivel;

            var sucesso = await _funcionarioRepository.UpdateAsync(id, funcionarioExistente);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao atualizar funcionário" });
            }

            // Publicar mensagem no RabbitMQ
            var message = new FuncionarioUpdatedMessage
            {
                FuncionarioId = funcionarioExistente.Id,
                Nome = funcionarioExistente.Nome,
                Email = funcionarioExistente.Email,
                Tipo = funcionarioExistente.Tipo,
                Nivel = funcionarioExistente.Nivel
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.FuncionarioUpdated);

            var response = new FuncionarioResponseDto
            {
                Id = funcionarioExistente.Id,
                Nome = funcionarioExistente.Nome,
                Email = funcionarioExistente.Email,
                Tipo = funcionarioExistente.Tipo,
                Nivel = funcionarioExistente.Nivel,
                DataCriacao = funcionarioExistente.DataCriacao
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
            if (funcionario == null)
            {
                return NotFound(new { message = "Funcionário não encontrado" });
            }

            var sucesso = await _funcionarioRepository.DeleteAsync(id);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao deletar funcionário" });
            }

            // Publicar mensagem no RabbitMQ
            var message = new FuncionarioDeletedMessage
            {
                FuncionarioId = funcionario.Id,
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Tipo = funcionario.Tipo
            };

            await _messageProducer.PublishAsync(message, _rabbitSettings.Queues.FuncionarioDeleted);

            return Ok(new { message = "Funcionário deletado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<ActionResult<FuncionarioResponseDto>> GetByEmail(string email)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByEmailAsync(email);
            
            if (funcionario == null)
            {
                return NotFound(new { message = "Funcionário não encontrado" });
            }

            var response = new FuncionarioResponseDto
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Email = funcionario.Email,
                Tipo = funcionario.Tipo,
                Nivel = funcionario.Nivel,
                DataCriacao = funcionario.DataCriacao
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("tipo/{tipo}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FuncionarioResponseDto>>> GetByTipo(string tipo)
    {
        try
        {
            var funcionarios = await _funcionarioRepository.GetByTypeAsync(tipo);
            
            var response = funcionarios.Select(f => new FuncionarioResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                Email = f.Email,
                Tipo = f.Tipo,
                Nivel = f.Nivel,
                DataCriacao = f.DataCriacao
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }
}