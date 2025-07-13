using Domain;

namespace Infrastructure.Repositories.Interfaces;

public interface IFuncionarioRepository
{
    Task<IEnumerable<Funcionario>> GetAllAsync();
    Task<Funcionario?> GetByIdAsync(string id);
    Task<Funcionario?> GetByEmailAsync(string email);
    Task<IEnumerable<Funcionario>> GetByTypeAsync(string tipo);
    Task<Funcionario> CreateAsync(Funcionario funcionario);
    Task<bool> UpdateAsync(string id, Funcionario funcionario);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsByEmailAsync(string email);
}