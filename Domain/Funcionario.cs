using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public abstract class Funcionario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("nome")]
    public string Nome { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("senha")]
    public string Senha { get; set; }
    
    [BsonElement("tipo")]
    public string Tipo { get; set; } // Para identificar o tipo (Gerente, etc.)
    
    [BsonElement("dataCriacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}