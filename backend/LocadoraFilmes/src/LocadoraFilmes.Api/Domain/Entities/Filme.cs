namespace LocadoraFilmes.Api.Domain.Entities;

public class Filme
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Sinopse { get; set; }
    public string? Diretor { get; set; }
    public int AnoLancamento { get; set; }
    public int DuracaoMinutos { get; set; }
    public string ClassificacaoIndicativa { get; set; } = string.Empty;
    public decimal PrecoDiaria { get; set; }
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public DateTimeOffset CriadoEm { get; set; }

    public int GeneroId { get; set; }
    public Genero Genero { get; set; } = null!;

    public ICollection<LocacaoItem> Itens { get; set; } = new List<LocacaoItem>();
}
