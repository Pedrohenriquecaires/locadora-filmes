namespace LocadoraFilmes.Validation;

public sealed class GeneroValidator : ValidadorBase<IGeneroDados>
{
    protected override void Regras(IGeneroDados genero, ResultadoValidacao resultado) =>
        ValidarTexto(resultado, nameof(IGeneroDados.Nome), genero.Nome, 2, 60);
}
