using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/filmes")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FilmesController : ControllerBase
{
    private readonly IFilmeService _service;

    public FilmesController(IFilmeService service)
    {
        _service = service;
    }

    /// <summary>Lista filmes, com filtros opcionais por título e gênero.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FilmeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FilmeResponse>>> Listar(
        [FromQuery] string? titulo, [FromQuery] int? generoId, CancellationToken ct)
        => Ok(await _service.ListarAsync(titulo, generoId, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FilmeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FilmeResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(FilmeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FilmeResponse>> Criar([FromBody] FilmeRequest request, CancellationToken ct)
    {
        var filme = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = filme.Id }, filme);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(FilmeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FilmeResponse>> Atualizar(int id, [FromBody] FilmeRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.ApenasAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
