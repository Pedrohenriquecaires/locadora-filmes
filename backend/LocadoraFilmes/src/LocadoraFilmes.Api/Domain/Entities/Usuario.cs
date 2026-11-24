namespace LocadoraFilmes.Api.Domain.Entities;

/// <summary>Usuário do sistema (funcionário da locadora) usado na autenticação.</summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public DateTimeOffset CriadoEm { get; set; }
}
