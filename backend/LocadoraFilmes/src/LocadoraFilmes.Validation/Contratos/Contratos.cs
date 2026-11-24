namespace LocadoraFilmes.Validation;

/// <summary>Dados de um gênero.</summary>
public interface IGeneroDados
{
    string? Nome { get; }
}

/// <summary>Dados de um filme.</summary>
public interface IFilmeDados
{
    string? Titulo { get; }
    string? Sinopse { get; }
    int AnoLancamento { get; }
    int DuracaoMinutos { get; }
    string? ClassificacaoIndicativa { get; }
    decimal PrecoDiaria { get; }
    int QuantidadeTotal { get; }
    int GeneroId { get; }
}

/// <summary>Dados de um cliente.</summary>
public interface IClienteDados
{
    string? Nome { get; }
    string? Cpf { get; }
    string? Email { get; }
    string? Telefone { get; }
    DateOnly? DataNascimento { get; }
}

/// <summary>Dados que definem o prazo de uma locação.</summary>
public interface IDiasLocacaoDados
{
    int DiasLocacao { get; }
}

/// <summary>Dados de uma nova locação.</summary>
public interface ILocacaoDados : IDiasLocacaoDados
{
    int ClienteId { get; }
    IReadOnlyList<int>? FilmeIds { get; }
}

/// <summary>Dados de cadastro de usuário do sistema.</summary>
public interface IUsuarioDados
{
    string? Nome { get; }
    string? Email { get; }
    string? Senha { get; }
    string? Perfil { get; }
}

/// <summary>Dados de login.</summary>
public interface ILoginDados
{
    string? Email { get; }
    string? Senha { get; }
}
