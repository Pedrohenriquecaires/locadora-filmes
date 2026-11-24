namespace LocadoraFilmes.Validation;

public sealed class FilmeValidator : ValidadorBase<IFilmeDados>
{
    public const int PrimeiroAnoDoCinema = 1888;

    public static readonly IReadOnlyCollection<string> ClassificacoesPermitidas =
        new[] { "L", "10", "12", "14", "16", "18" };

    private readonly Func<int> _anoAtual;

    public FilmeValidator() : this(() => DateTime.UtcNow.Year)
    {
    }

    public FilmeValidator(Func<int> anoAtual)
    {
        _anoAtual = anoAtual;
    }

    protected override void Regras(IFilmeDados filme, ResultadoValidacao resultado)
    {
        ValidarTexto(resultado, nameof(IFilmeDados.Titulo), filme.Titulo, 1, 150);
        ValidarTexto(resultado, nameof(IFilmeDados.Sinopse), filme.Sinopse, 0, 1000, obrigatorio: false);

        var anoMaximo = _anoAtual() + 1;
        if (filme.AnoLancamento < PrimeiroAnoDoCinema || filme.AnoLancamento > anoMaximo)
        {
            resultado.Adicionar(nameof(IFilmeDados.AnoLancamento),
                $"O ano de lançamento deve estar entre {PrimeiroAnoDoCinema} e {anoMaximo}.");
        }

        if (filme.DuracaoMinutos < 1 || filme.DuracaoMinutos > 600)
        {
            resultado.Adicionar(nameof(IFilmeDados.DuracaoMinutos), "A duração deve estar entre 1 e 600 minutos.");
        }

        var classificacao = filme.ClassificacaoIndicativa?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(classificacao) || !ClassificacoesPermitidas.Contains(classificacao))
        {
            resultado.Adicionar(nameof(IFilmeDados.ClassificacaoIndicativa),
                $"A classificação indicativa deve ser uma das opções: {string.Join(", ", ClassificacoesPermitidas)}.");
        }

        if (filme.PrecoDiaria <= 0 || filme.PrecoDiaria > 999.99m)
        {
            resultado.Adicionar(nameof(IFilmeDados.PrecoDiaria), "O preço da diária deve ser maior que zero e no máximo 999,99.");
        }

        if (filme.QuantidadeTotal < 0 || filme.QuantidadeTotal > 1000)
        {
            resultado.Adicionar(nameof(IFilmeDados.QuantidadeTotal), "A quantidade total deve estar entre 0 e 1000.");
        }

        if (filme.GeneroId <= 0)
        {
            resultado.Adicionar(nameof(IFilmeDados.GeneroId), "Informe um gênero válido.");
        }
    }
}
