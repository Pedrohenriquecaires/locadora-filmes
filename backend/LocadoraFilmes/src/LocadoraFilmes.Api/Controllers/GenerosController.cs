using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/generos")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class GenerosController : ControllerBase
{
    private readonly IGeneroService _service;

    public GenerosController(IGeneroService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GeneroResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GeneroResponse>>> Listar(CancellationToken ct)
        => Ok(await _service.ListarAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GeneroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeneroResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(GeneroResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GeneroResponse>> Criar([FromBody] GeneroRequest request, CancellationToken ct)
    {
        var genero = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = genero.Id }, genero);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(GeneroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GeneroResponse>> Atualizar(int id, [FromBody] GeneroRequest request, CancellationToken ct)
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
