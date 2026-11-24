namespace LocadoraFilmes.Api.Dtos;

public sealed record FilmeRankingResponse(string Titulo, int Quantidade);

public sealed record ClienteRankingResponse(int ClienteId, string Nome, int Quantidade);

public sealed record AlugueisMesResponse(string Mes, int Quantidade);

public sealed record DashboardResponse(
    int TotalClientes,
    int TotalFilmes,
    int LocacoesAtivas,
    int LocacoesAtrasadas,
    decimal ReceitaTotal,
    IReadOnlyList<FilmeRankingResponse> TopFilmes,
    IReadOnlyList<ClienteRankingResponse> RankingClientes,
    IReadOnlyList<AlugueisMesResponse> AlugueisPorMes);