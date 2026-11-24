using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service)
    {
        _service = service;
    }

    /// <summary>Lista clientes, com filtro opcional por nome.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> Listar([FromQuery] string? nome, CancellationToken ct)
        => Ok(await _service.ListarAsync(nome, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Criar([FromBody] ClienteRequest request, CancellationToken ct)
    {
        var cliente = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.AdminOuAtendente)]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Atualizar(int id, [FromBody] ClienteRequest request, CancellationToken ct)
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
