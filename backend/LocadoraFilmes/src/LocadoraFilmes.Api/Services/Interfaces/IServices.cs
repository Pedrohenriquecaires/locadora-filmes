using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Api.Dtos;

namespace LocadoraFilmes.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> ObterUsuarioAsync(int id, CancellationToken ct = default);
}

public interface IGeneroService
{
    Task<IReadOnlyList<GeneroResponse>> ListarAsync(CancellationToken ct = default);
    Task<GeneroResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<GeneroResponse> CriarAsync(GeneroRequest request, CancellationToken ct = default);
    Task<GeneroResponse> AtualizarAsync(int id, GeneroRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface IFilmeService
{
    Task<IReadOnlyList<FilmeResponse>> ListarAsync(string? titulo, int? generoId, CancellationToken ct = default);
    Task<FilmeResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<FilmeResponse> CriarAsync(FilmeRequest request, CancellationToken ct = default);
    Task<FilmeResponse> AtualizarAsync(int id, FilmeRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface IClienteService
{
    Task<IReadOnlyList<ClienteResponse>> ListarAsync(string? nome, CancellationToken ct = default);
    Task<ClienteResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<ClienteResponse> CriarAsync(ClienteRequest request, CancellationToken ct = default);
    Task<ClienteResponse> AtualizarAsync(int id, ClienteRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface ILocacaoService
{
    Task<IReadOnlyList<LocacaoResponse>> ListarAsync(int? clienteId, StatusLocacao? status, CancellationToken ct = default);
    Task<LocacaoResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<LocacaoResponse> CriarAsync(CriarLocacaoRequest request, CancellationToken ct = default);
    Task<LocacaoResponse> AtualizarAsync(int id, AtualizarLocacaoRequest request, CancellationToken ct = default);
    Task<LocacaoResponse> DevolverAsync(int id, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}
