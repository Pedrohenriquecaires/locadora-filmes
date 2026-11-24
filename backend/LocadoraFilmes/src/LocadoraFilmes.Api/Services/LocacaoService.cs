using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Services.Interfaces;
using LocadoraFilmes.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

public class LocacaoService : ILocacaoService
{
    /// <summary>Máximo de locações ativas simultâneas por cliente.</summary>
    public const int MaximoLocacoesAtivasPorCliente = 3;

    /// <summary>Multa por dia de atraso: 50% da soma das diárias dos filmes locados.</summary>
    public const decimal FatorMultaDiaria = 0.5m;

    private readonly LocadoraDbContext _db;
    private readonly IValidator<ILocacaoDados> _validator;
    private readonly IValidator<IDiasLocacaoDados> _validatorDias;

    public LocacaoService(
        LocadoraDbContext db,
        IValidator<ILocacaoDados> validator,
        IValidator<IDiasLocacaoDados> validatorDias)
    {
        _db = db;
        _validator = validator;
        _validatorDias = validatorDias;
    }

    public async Task<IReadOnlyList<LocacaoResponse>> ListarAsync(
        int? clienteId, StatusLocacao? status, CancellationToken ct = default)
    {
        var consulta = ConsultaCompleta().AsNoTracking();

        if (clienteId.HasValue)
        {
            consulta = consulta.Where(l => l.ClienteId == clienteId.Value);
        }

        if (status.HasValue)
        {
            consulta = consulta.Where(l => l.Status == status.Value);
        }

        var locacoes = await consulta.OrderByDescending(l => l.Id).ToListAsync(ct);
        return locacoes.Select(l => l.ToResponse()).ToList();
    }

    public async Task<LocacaoResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var locacao = await ConsultaCompleta().AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Locação {id} não encontrada.");

        return locacao.ToResponse();
    }

    public async Task<LocacaoResponse> CriarAsync(CriarLocacaoRequest request, CancellationToken ct = default)
    {
        _validator.Validar(request).LancarSeInvalido();

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == request.ClienteId, ct)
            ?? throw new ValidacaoException(nameof(request.ClienteId), "Cliente não encontrado.");

        var ids = request.FilmeIds!.ToList();
        var filmes = await _db.Filmes.Where(f => ids.Contains(f.Id)).ToListAsync(ct);

        var inexistentes = ids.Except(filmes.Select(f => f.Id)).ToList();
        if (inexistentes.Count > 0)
        {
            throw new ValidacaoException(
                nameof(request.FilmeIds), $"Filme(s) não encontrado(s): {string.Join(", ", inexistentes)}.");
        }

        var agora = DateTimeOffset.UtcNow;

        var ativas = await _db.Locacoes
            .Where(l => l.ClienteId == cliente.Id && l.Status == StatusLocacao.Ativa)
            .ToListAsync(ct);

        if (ativas.Any(l => l.DataPrevistaDevolucao < agora))
        {
            throw new RegraNegocioException("O cliente possui locação em atraso e não pode realizar novas locações.");
        }

        if (ativas.Count >= MaximoLocacoesAtivasPorCliente)
        {
            throw new RegraNegocioException(
                $"O cliente já possui {MaximoLocacoesAtivasPorCliente} locações ativas (limite máximo).");
        }

        var semEstoque = filmes.Where(f => f.QuantidadeDisponivel < 1).Select(f => f.Titulo).ToList();
        if (semEstoque.Count > 0)
        {
            throw new ConflitoException($"Sem estoque disponível para: {string.Join(", ", semEstoque)}.");
        }

        var locacao = new Locacao
        {
            Cliente = cliente,
            DataLocacao = agora,
            DataPrevistaDevolucao = agora.AddDays(request.DiasLocacao),
            Status = StatusLocacao.Ativa,
            ValorMulta = 0m
        };

        foreach (var filme in filmes)
        {
            filme.QuantidadeDisponivel--;
            locacao.Itens.Add(new LocacaoItem { Filme = filme, PrecoDiaria = filme.PrecoDiaria });
        }

        locacao.ValorTotal = CalcularValor(locacao, request.DiasLocacao);

        _db.Locacoes.Add(locacao);
        await _db.SaveChangesAsync(ct);

        return locacao.ToResponse();
    }

    public async Task<LocacaoResponse> AtualizarAsync(int id, AtualizarLocacaoRequest request, CancellationToken ct = default)
    {
        var locacao = await ConsultaCompleta().FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Locação {id} não encontrada.");

        _validatorDias.Validar(request).LancarSeInvalido();

        if (locacao.Status != StatusLocacao.Ativa)
        {
            throw new ConflitoException("Somente locações ativas podem ser alteradas.");
        }

        locacao.DataPrevistaDevolucao = locacao.DataLocacao.AddDays(request.DiasLocacao);
        locacao.ValorTotal = CalcularValor(locacao, request.DiasLocacao);

        await _db.SaveChangesAsync(ct);

        return locacao.ToResponse();
    }

    public async Task<LocacaoResponse> DevolverAsync(int id, CancellationToken ct = default)
    {
        var locacao = await ConsultaCompleta().FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Locação {id} não encontrada.");

        if (locacao.Status == StatusLocacao.Devolvida)
        {
            throw new ConflitoException("Esta locação já foi devolvida.");
        }

        var agora = DateTimeOffset.UtcNow;
        var diasAtraso = 0;

        if (agora > locacao.DataPrevistaDevolucao)
        {
            diasAtraso = (int)Math.Ceiling((agora - locacao.DataPrevistaDevolucao).TotalDays);
        }

        var somaDiarias = locacao.Itens.Sum(i => i.PrecoDiaria);

        locacao.ValorMulta = Arredondar(somaDiarias * diasAtraso * FatorMultaDiaria);
        locacao.DataDevolucao = agora;
        locacao.Status = StatusLocacao.Devolvida;

        DevolverEstoque(locacao);

        await _db.SaveChangesAsync(ct);

        return locacao.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var locacao = await ConsultaCompleta().FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Locação {id} não encontrada.");

        if (locacao.Status == StatusLocacao.Ativa)
        {
            DevolverEstoque(locacao);
        }

        _db.Locacoes.Remove(locacao);
        await _db.SaveChangesAsync(ct);
    }

    private IQueryable<Locacao> ConsultaCompleta() =>
        _db.Locacoes
            .Include(l => l.Cliente)
            .Include(l => l.Itens)
            .ThenInclude(i => i.Filme);

    private static decimal CalcularValor(Locacao locacao, int dias) =>
        Arredondar(locacao.Itens.Sum(i => i.PrecoDiaria) * dias);

    private static decimal Arredondar(decimal valor) =>
        decimal.Round(valor, 2, MidpointRounding.AwayFromZero);

    private static void DevolverEstoque(Locacao locacao)
    {
        foreach (var item in locacao.Itens)
        {
            item.Filme.QuantidadeDisponivel =
                Math.Min(item.Filme.QuantidadeTotal, item.Filme.QuantidadeDisponivel + 1);
        }
    }
}
