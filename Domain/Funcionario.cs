using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public class Funcionario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    
    [BsonElement("senha")]
    public string Senha { get; set; } = string.Empty;
    
    [BsonElement("tipo")]
    public string Tipo { get; set; } = string.Empty; // "Gerente", "Funcionario", etc.
    
    [BsonElement("nivel")]
    public int Nivel { get; set; } = 1; // Campo que antes estava apenas em Gerente
    
    [BsonElement("dataCriacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}