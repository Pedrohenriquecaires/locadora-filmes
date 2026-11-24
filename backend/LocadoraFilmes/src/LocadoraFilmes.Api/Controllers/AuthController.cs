using System.Security.Claims;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraFilmes.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Autentica o usuário e devolve um token JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        => Ok(await _service.LoginAsync(request, ct));

    /// <summary>Cria um novo usuário do sistema (somente Admin).</summary>
    [HttpPost("registrar")]
    [Authorize(Policy = Politicas.ApenasAdmin)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar([FromBody] RegistrarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _service.RegistrarAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, usuario);
    }

    /// <summary>Retorna os dados do usuário autenticado.</summary>
    [HttpGet("eu")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> Eu(CancellationToken ct)
    {
        var valor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(valor, out var id))
        {
            return Unauthorized();
        }

        return Ok(await _service.ObterUsuarioAsync(id, ct));
    }
}
