namespace LocadoraFilmes.Validation;

/// <summary>Representa um erro de validação associado a um campo.</summary>
public sealed record ErroValidacao(string Campo, string Mensagem);

/// <summary>Resultado de uma validação: lista de erros (vazia = válido).</summary>
public sealed class ResultadoValidacao
{
    private readonly List<ErroValidacao> _erros = new();

    public bool EhValido => _erros.Count == 0;

    public IReadOnlyList<ErroValidacao> Erros => _erros;

    public ResultadoValidacao Adicionar(string campo, string mensagem)
    {
        _erros.Add(new ErroValidacao(campo, mensagem));
        return this;
    }

    public ResultadoValidacao Adicionar(ResultadoValidacao outro)
    {
        _erros.AddRange(outro.Erros);
        return this;
    }

    /// <summary>Agrupa as mensagens por campo (formato usado por ProblemDetails de validação).</summary>
    public IDictionary<string, string[]> ParaDicionario() =>
        _erros
            .GroupBy(e => e.Campo)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Mensagem).ToArray());

    /// <summary>Lança <see cref="ValidacaoException"/> se houver erros.</summary>
    public void LancarSeInvalido()
    {
        if (!EhValido)
        {
            throw new ValidacaoException(this);
        }
    }
}
