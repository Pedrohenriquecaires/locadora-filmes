namespace LocadoraFilmes.Validation;

/// <summary>Exceção lançada quando uma validação falha.</summary>
public sealed class ValidacaoException : Exception
{
    public ResultadoValidacao Resultado { get; }

    public ValidacaoException(ResultadoValidacao resultado)
        : base("Um ou mais erros de validação ocorreram.")
    {
        Resultado = resultado;
    }

    public ValidacaoException(string campo, string mensagem)
        : this(new ResultadoValidacao().Adicionar(campo, mensagem))
    {
    }
}
