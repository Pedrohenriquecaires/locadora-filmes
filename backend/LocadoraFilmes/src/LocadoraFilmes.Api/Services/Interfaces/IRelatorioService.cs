using LocadoraFilmes.Api.Dtos;

namespace LocadoraFilmes.Api.Services.Interfaces;

public interface IRelatorioService
{
    Task<DashboardResponse> ObterDashboardAsync(CancellationToken ct = default);
}