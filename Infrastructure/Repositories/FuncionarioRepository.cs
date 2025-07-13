using MongoDB.Driver;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly IMongoCollection<Funcionario> _funcionarios;

    public FuncionarioRepository(MongoDbContext context)
    {
        _funcionarios = context.Funcionarios;
    }

    public async Task<IEnumerable<Funcionario>> GetAllAsync()
    {
        return await _funcionarios.Find(_ => true).ToListAsync();
    }

    public async Task<Funcionario?> GetByIdAsync(string id)
    {
        return await _funcionarios.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Funcionario?> GetByEmailAsync(string email)
    {
        return await _funcionarios.Find(f => f.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Funcionario> CreateAsync(Funcionario funcionario)
    {
        await _funcionarios.InsertOneAsync(funcionario);
        return funcionario;
    }

    public async Task<bool> UpdateAsync(string id, Funcionario funcionario)
    {
        var result = await _funcionarios.ReplaceOneAsync(f => f.Id == id, funcionario);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _funcionarios.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var count = await _funcionarios.CountDocumentsAsync(f => f.Email == email);
        return count > 0;
    }

    public async Task<IEnumerable<Funcionario>> GetByTypeAsync(string tipo)
    {
        return await _funcionarios.Find(f => f.Tipo == tipo).ToListAsync();
    }
}