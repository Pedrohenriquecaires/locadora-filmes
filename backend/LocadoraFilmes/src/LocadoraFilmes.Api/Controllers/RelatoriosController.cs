using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _service;

    public RelatoriosController(IRelatorioService service)
    {
        _service = service;
    }

    /// <summary>Indicadores gerais, top filmes, ranking de clientes e aluguéis dos últimos 6 meses.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Dashboard(CancellationToken ct)
        => Ok(await _service.ObterDashboardAsync(ct));
}