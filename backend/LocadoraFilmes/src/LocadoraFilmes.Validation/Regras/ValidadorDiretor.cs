namespace LocadoraFilmes.Validation;

/// <summary>
/// Validação do nome do diretor de um filme (adicionada na versão 1.1.0).
/// O campo é opcional; quando informado deve ter de 3 a 120 caracteres
/// e conter apenas letras, espaços, ponto, apóstrofo ou hífen.
/// </summary>
public static class ValidadorDiretor
{
    public const int TamanhoMinimo = 3;
    public const int TamanhoMaximo = 120;

    public static bool EhValido(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return true; // opcional
        }

        var texto = nome.Trim();

        if (texto.Length < TamanhoMinimo || texto.Length > TamanhoMaximo)
        {
            return false;
        }

        if (texto.Count(char.IsLetter) < 2)
        {
            return false;
        }

        return texto.All(c => char.IsLetter(c) || c is ' ' or '.' or '\'' or '-');
    }
}