namespace AuthService.DTO;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public GerenteResponseDto Usuario { get; set; } = new();
}