using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/locacoes")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class LocacoesController : ControllerBase
{
    private readonly ILocacaoService _service;

    public LocacoesController(ILocacaoService service)
    {
        _service = service;
    }

    /// <summary>Lista locações, com filtros opcionais por cliente e status (Ativa/Devolvida).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LocacaoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LocacaoResponse>>> Listar(
        [FromQuery] int? clienteId, [FromQuery] StatusLocacao? status, CancellationToken ct)
        => Ok(await _service.ListarAsync(clienteId, status, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocacaoResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Registra uma nova locação (baixa o estoque dos filmes).</summary>
    [HttpPost]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LocacaoResponse>> Criar([FromBody] CriarLocacaoRequest request, CancellationToken ct)
    {
        var locacao = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = locacao.Id }, locacao);
    }

    /// <summary>Altera o prazo de uma locação ativa.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocacaoResponse>> Atualizar(
        int id, [FromBody] AtualizarLocacaoRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Registra a devolução (calcula multa por atraso e devolve o estoque).</summary>
    [HttpPost("{id:int}/devolucao")]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocacaoResponse>> Devolver(int id, CancellationToken ct)
        => Ok(await _service.DevolverAsync(id, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.ApenasAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
