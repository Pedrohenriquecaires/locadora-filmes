namespace LocadoraFilmes.Api.Domain.Entities;

/// <summary>Filme incluído em uma locação (guarda o preço da diária no momento da locação).</summary>
public class LocacaoItem
{
    public int Id { get; set; }

    public int LocacaoId { get; set; }
    public Locacao Locacao { get; set; } = null!;

    public int FilmeId { get; set; }
    public Filme Filme { get; set; } = null!;

    public decimal PrecoDiaria { get; set; }
}
