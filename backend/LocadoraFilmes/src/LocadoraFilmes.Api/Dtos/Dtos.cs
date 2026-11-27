using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Validation;

namespace LocadoraFilmes.Api.Dtos;

// ---------- Autenticação ----------
public sealed record LoginRequest(string? Email, string? Senha) : ILoginDados;

public sealed record RegistrarUsuarioRequest(string? Nome, string? Email, string? Senha, string? Perfil) : IUsuarioDados;

public sealed record UsuarioResponse(int Id, string Nome, string Email, string Perfil);

public sealed record LoginResponse(string Token, string TipoToken, DateTimeOffset ExpiraEm, UsuarioResponse Usuario);

// ---------- Gêneros ----------
public sealed record GeneroRequest(string? Nome) : IGeneroDados;

public sealed record GeneroResponse(int Id, string Nome);

// ---------- Filmes ----------
public sealed record FilmeRequest(
    string? Titulo,
    string? Sinopse,
    string? Diretor,
    int AnoLancamento,
    int DuracaoMinutos,
    string? ClassificacaoIndicativa,
    decimal PrecoDiaria,
    int QuantidadeTotal,
    int GeneroId) : IFilmeDados;

public sealed record FilmeResponse(
    int Id,
    string Titulo,
    string? Sinopse,
    string? Diretor,
    int AnoLancamento,
    int DuracaoMinutos,
    string ClassificacaoIndicativa,
    decimal PrecoDiaria,
    int QuantidadeTotal,
    int QuantidadeDisponivel,
    int GeneroId,
    string Genero);

// ---------- Clientes ----------
public sealed record ClienteRequest(
    string? Nome,
    string? Cpf,
    string? Email,
    string? Telefone,
    DateOnly? DataNascimento) : IClienteDados;

public sealed record ClienteResponse(
    int Id,
    string Nome,
    string Cpf,
    string Email,
    string? Telefone,
    DateOnly DataNascimento);

// ---------- Locações ----------
public sealed record CriarLocacaoRequest(
    int ClienteId,
    IReadOnlyList<int>? FilmeIds,
    int DiasLocacao) : ILocacaoDados;

public sealed record AtualizarLocacaoRequest(int DiasLocacao) : IDiasLocacaoDados;

public sealed record LocacaoItemResponse(int FilmeId, string Titulo, decimal PrecoDiaria);

public sealed record LocacaoResponse(
    int Id,
    int ClienteId,
    string ClienteNome,
    DateTimeOffset DataLocacao,
    DateTimeOffset DataPrevistaDevolucao,
    DateTimeOffset? DataDevolucao,
    decimal ValorTotal,
    decimal ValorMulta,
    StatusLocacao Status,
    IReadOnlyList<LocacaoItemResponse> Itens);
