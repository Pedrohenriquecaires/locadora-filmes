namespace LocadoraFilmes.Validation;

/// <summary>Contrato de um validador para o tipo <typeparamref name="T"/>.</summary>
public interface IValidator<in T>
{
    ResultadoValidacao Validar(T instancia);
}
