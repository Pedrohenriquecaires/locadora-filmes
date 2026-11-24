using System.Globalization;
using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

/// <summary>
/// Relatório gerencial da locadora (indicadores, rankings e aluguéis por mês).
/// Os cálculos com decimal/DateTimeOffset são feitos em memória por limitação do SQLite.
/// </summary>
public class RelatorioService : IRelatorioService
{
    private const int TamanhoRanking = 5;
    private const int MesesNoGrafico = 6;

    private readonly LocadoraDbContext _db;

    public RelatorioService(LocadoraDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardResponse> ObterDashboardAsync(CancellationToken ct = default)
    {
        var totalClientes = await _db.Clientes.CountAsync(ct);
        var totalFilmes = await _db.Filmes.CountAsync(ct);

        var locacoes = await _db.Locacoes
            .AsNoTracking()
            .Include(l => l.Cliente)
            .Include(l => l.Itens).ThenInclude(i => i.Filme)
            .ToListAsync(ct);

        var agora = DateTimeOffset.UtcNow;

        // Locação "atrasada" = ainda ativa e com a data prevista de devolução vencida.
        var ativas = locacoes.Where(l => l.Status == StatusLocacao.Ativa).ToList();
        var atrasadas = ativas.Count(l => l.DataPrevistaDevolucao < agora);
        var ativasEmDia = ativas.Count - atrasadas;

        // Receita = valor das locações devolvidas + multas cobradas.
        var receitaTotal = locacoes
            .Where(l => l.Status == StatusLocacao.Devolvida)
            .Sum(l => l.ValorTotal + l.ValorMulta);

        var topFilmes = locacoes
            .SelectMany(l => l.Itens)
            .GroupBy(i => i.Filme.Titulo)
            .Select(g => new FilmeRankingResponse(g.Key, g.Count()))
            .OrderByDescending(f => f.Quantidade)
            .ThenBy(f => f.Titulo)
            .Take(TamanhoRanking)
            .ToList();

        var rankingClientes = locacoes
            .GroupBy(l => new { l.ClienteId, l.Cliente.Nome })
            .Select(g => new ClienteRankingResponse(g.Key.ClienteId, g.Key.Nome, g.Sum(l => l.Itens.Count)))
            .OrderByDescending(c => c.Quantidade)
            .ThenBy(c => c.Nome)
            .Take(TamanhoRanking)
            .ToList();

        var cultura = new CultureInfo("pt-BR");
        var inicioMesAtual = new DateTime(agora.Year, agora.Month, 1);

        var alugueisPorMes = Enumerable.Range(0, MesesNoGrafico)
            .Select(i => inicioMesAtual.AddMonths(i - (MesesNoGrafico - 1)))
            .Select(mes => new AlugueisMesResponse(
                cultura.DateTimeFormat.GetAbbreviatedMonthName(mes.Month).Replace(".", string.Empty),
                locacoes
                    .Where(l => l.DataLocacao.UtcDateTime.Year == mes.Year && l.DataLocacao.UtcDateTime.Month == mes.Month)
                    .Sum(l => l.Itens.Count)))
            .ToList();

        return new DashboardResponse(
            totalClientes,
            totalFilmes,
            ativasEmDia,
            atrasadas,
            receitaTotal,
            topFilmes,
            rankingClientes,
            alugueisPorMes);
    }
}