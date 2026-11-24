namespace LocadoraFilmes.Api.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Somente dígitos (11).</summary>
    public string Cpf { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Somente dígitos (10 ou 11).</summary>
    public string? Telefone { get; set; }

    public DateOnly DataNascimento { get; set; }
    public DateTimeOffset CriadoEm { get; set; }

    public ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
}
