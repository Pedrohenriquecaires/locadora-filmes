using LocadoraFilmes.Validation;

namespace LocadoraFilmes.Validation.Tests.Fakes;

public sealed record GeneroFake : IGeneroDados
{
    public string? Nome { get; init; } = "Ficção Científica";
}

public sealed record FilmeFake : IFilmeDados
{
    public string? Titulo { get; init; } = "Matrix";
    public string? Sinopse { get; init; } = "Um programador descobre a verdade sobre a realidade.";
    public int AnoLancamento { get; init; } = 1999;
    public int DuracaoMinutos { get; init; } = 136;
    public string? ClassificacaoIndicativa { get; init; } = "14";
    public decimal PrecoDiaria { get; init; } = 6.5m;
    public int QuantidadeTotal { get; init; } = 5;
    public int GeneroId { get; init; } = 1;
}

public sealed record ClienteFake : IClienteDados
{
    public string? Nome { get; init; } = "Ana Souza";
    public string? Cpf { get; init; } = "529.982.247-25";
    public string? Email { get; init; } = "ana@email.com";
    public string? Telefone { get; init; } = "(11) 98765-4321";
    public DateOnly? DataNascimento { get; init; } = new DateOnly(1990, 5, 12);
}

public sealed record LocacaoFake : ILocacaoDados
{
    public int ClienteId { get; init; } = 1;
    public IReadOnlyList<int>? FilmeIds { get; init; } = new[] { 1, 2 };
    public int DiasLocacao { get; init; } = 3;
}

public sealed record DiasFake(int DiasLocacao) : IDiasLocacaoDados;

public sealed record UsuarioFake : IUsuarioDados
{
    public string? Nome { get; init; } = "Maria Atendente";
    public string? Email { get; init; } = "maria@locadora.com";
    public string? Senha { get; init; } = "Senha@123";
    public string? Perfil { get; init; } = "Atendente";
}

public sealed record LoginFake : ILoginDados
{
    public string? Email { get; init; } = "maria@locadora.com";
    public string? Senha { get; init; } = "Senha@123";
}
