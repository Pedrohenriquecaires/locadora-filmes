using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using LocadoraFilmes.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

public class AuthService : IAuthService
{
    private readonly LocadoraDbContext _db;
    private readonly ISenhaHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IValidator<ILoginDados> _loginValidator;
    private readonly IValidator<IUsuarioDados> _usuarioValidator;

    public AuthService(
        LocadoraDbContext db,
        ISenhaHasher hasher,
        ITokenService tokens,
        IValidator<ILoginDados> loginValidator,
        IValidator<IUsuarioDados> usuarioValidator)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
        _loginValidator = loginValidator;
        _usuarioValidator = usuarioValidator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        _loginValidator.Validar(request).LancarSeInvalido();

        var email = request.Email!.Trim().ToLowerInvariant();
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (usuario is null || !_hasher.Verificar(request.Senha!, usuario.SenhaHash))
        {
            throw new CredenciaisInvalidasException();
        }

        var (token, expiraEm) = _tokens.Gerar(usuario);
        return new LoginResponse(token, "Bearer", expiraEm, usuario.ToResponse());
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken ct = default)
    {
        _usuarioValidator.Validar(request).LancarSeInvalido();

        var email = request.Email!.Trim().ToLowerInvariant();

        if (await _db.Usuarios.AnyAsync(u => u.Email == email, ct))
        {
            throw new ConflitoException($"Já existe um usuário com o e-mail '{email}'.");
        }

        var usuario = new Usuario
        {
            Nome = request.Nome!.Trim(),
            Email = email,
            SenhaHash = _hasher.Gerar(request.Senha!),
            Perfil = Perfis.Normalizar(request.Perfil)!,
            CriadoEm = DateTimeOffset.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync(ct);

        return usuario.ToResponse();
    }

    public async Task<UsuarioResponse> ObterUsuarioAsync(int id, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        return usuario.ToResponse();
    }
}
