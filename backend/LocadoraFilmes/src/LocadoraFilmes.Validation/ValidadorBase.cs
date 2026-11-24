namespace LocadoraFilmes.Validation;

/// <summary>Classe base com utilitários comuns para os validadores.</summary>
public abstract class ValidadorBase<T> : IValidator<T>
{
    public ResultadoValidacao Validar(T instancia)
    {
        var resultado = new ResultadoValidacao();

        if (instancia is null)
        {
            resultado.Adicionar(string.Empty, "O objeto a ser validado não pode ser nulo.");
            return resultado;
        }

        Regras(instancia, resultado);
        return resultado;
    }

    protected abstract void Regras(T instancia, ResultadoValidacao resultado);

    protected static void ValidarTexto(
        ResultadoValidacao resultado, string campo, string? valor, int minimo, int maximo, bool obrigatorio = true)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            if (obrigatorio)
            {
                resultado.Adicionar(campo, $"O campo {campo} é obrigatório.");
            }

            return;
        }

        var tamanho = valor.Trim().Length;

        if (tamanho < minimo)
        {
            resultado.Adicionar(campo, $"O campo {campo} deve ter no mínimo {minimo} caracteres.");
        }

        if (tamanho > maximo)
        {
            resultado.Adicionar(campo, $"O campo {campo} deve ter no máximo {maximo} caracteres.");
        }
    }
}
