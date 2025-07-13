using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository;

public class GerenteRepository : IGerenteRepository
{
    private readonly AuthContext _context;
    
    public GerenteRepository(AuthContext context)
    {
        _context = context; 
    }

    public async Task<Gerente> Create(Gerente gerente)
    {
         _context.Gerentes.AddAsync(gerente);
         await _context.SaveChangesAsync();
         return gerente;
    }

    public async Task<Gerente> FindById(int id)
    {
        return await _context.Gerentes.FindAsync(id);
    }

    public async Task<List<Gerente>> FindAll()
    {
        return await _context.Gerentes.ToListAsync();
    }

    public async Task<Gerente> Update(Gerente gerente)
    {
        var retorno = await FindById(gerente.Id);
        
        if(retorno == null)
            return null;
        
        retorno.Email = gerente.Email;
        retorno.Nome = gerente.Nome;
        retorno.Senha = gerente.Senha;
        
        _context.Gerentes.Update(retorno);
        _context.SaveChanges();
        
        return retorno;
    }

    public async Task<Task> Delete(int id)
    {
        _context.Gerentes.Remove(await _context.Gerentes.FindAsync(id));
        await _context.SaveChangesAsync();
        return Task.CompletedTask;
    }
}