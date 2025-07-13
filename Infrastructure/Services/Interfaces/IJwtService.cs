using Domain;

namespace Infrastructure.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(Funcionario funcionario);
    bool ValidateToken(string token);
}