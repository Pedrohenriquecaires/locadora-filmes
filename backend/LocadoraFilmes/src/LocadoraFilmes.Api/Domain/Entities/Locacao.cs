using LocadoraFilmes.Api.Domain.Enums;

namespace LocadoraFilmes.Api.Domain.Entities;

public class Locacao
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public DateTimeOffset DataLocacao { get; set; }
    public DateTimeOffset DataPrevistaDevolucao { get; set; }
    public DateTimeOffset? DataDevolucao { get; set; }

    public decimal ValorTotal { get; set; }
    public decimal ValorMulta { get; set; }
    public StatusLocacao Status { get; set; }

    public ICollection<LocacaoItem> Itens { get; set; } = new List<LocacaoItem>();
}
