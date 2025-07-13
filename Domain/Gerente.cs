using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

[BsonDiscriminator("Gerente")]
public class Gerente : Funcionario
{
    public Gerente()
    {
        Tipo = "Gerente";
    }
    
    // Propriedades específicas do Gerente podem ser adicionadas aqui
    [BsonElement("nivel")]
    public int Nivel { get; set; } = 1;
}