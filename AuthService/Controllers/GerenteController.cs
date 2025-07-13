using AuthService.DTO;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GerenteController : ControllerBase
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IPasswordService _passwordService;

    public GerenteController(
        IFuncionarioRepository funcionarioRepository,
        IPasswordService passwordService)
    {
        _funcionarioRepository = funcionarioRepository;
        _passwordService = passwordService;
    }

    [HttpGet]
    [Authorize] // Proteger apenas este endpoint
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
    [Authorize] // Proteger apenas este endpoint
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
    // Removido [Authorize] - endpoint público para criação
    public async Task<ActionResult<GerenteResponseDto>> Create([FromBody] GerenteCreateDto gerenteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se email já existe
            if (await _funcionarioRepository.ExistsByEmailAsync(gerenteDto.Email))
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            // Criar novo gerente
            var gerente = new Gerente
            {
                Nome = gerenteDto.Nome,
                Email = gerenteDto.Email,
                Senha = _passwordService.HashPassword(gerenteDto.Senha),
                Nivel = gerenteDto.Nivel
            };

            var gerenteCriado = await _funcionarioRepository.CreateAsync(gerente);
            var gerenteResponse = (Gerente)gerenteCriado;

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
    [Authorize] // Proteger apenas este endpoint
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

            // Verificar se email já existe (exceto o próprio)
            var funcionarioComEmail = await _funcionarioRepository.GetByEmailAsync(gerenteDto.Email);
            if (funcionarioComEmail != null && funcionarioComEmail.Id != id)
            {
                return Conflict(new { message = "Email já está em uso" });
            }

            // Atualizar dados
            gerenteExistente.Nome = gerenteDto.Nome;
            gerenteExistente.Email = gerenteDto.Email;
            gerenteExistente.Nivel = gerenteDto.Nivel;

            var sucesso = await _funcionarioRepository.UpdateAsync(id, gerenteExistente);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao atualizar gerente" });
            }

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
    [Authorize] // Proteger apenas este endpoint
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionario == null || funcionario.Tipo != "Gerente")
            {
                return NotFound(new { message = "Gerente não encontrado" });
            }

            var sucesso = await _funcionarioRepository.DeleteAsync(id);
            
            if (!sucesso)
            {
                return BadRequest(new { message = "Erro ao deletar gerente" });
            }

            return Ok(new { message = "Gerente deletado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet("email/{email}")]
    [Authorize] // Proteger apenas este endpoint
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