using MongoDB.Driver;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Domain;

namespace Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Funcionario> Funcionarios => 
        _database.GetCollection<Funcionario>(_settings.FuncionariosCollectionName);
    
    public IMongoCollection<Gerente> Gerentes => 
        _database.GetCollection<Gerente>(_settings.FuncionariosCollectionName);
}