using Domain;

namespace Data.Repository;

public interface IGerenteRepository
{
    Task<Gerente> Create(Gerente gerente);
    Task<Gerente> FindById(int id);
    Task<List<Gerente>> FindAll();
    Task<Gerente> Update(Gerente gerente);
    Task<Task> Delete(int id);
}