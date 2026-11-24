using System.Text.RegularExpressions;

namespace LocadoraFilmes.Validation;

/// <summary>Validação de e-mail.</summary>
public static class ValidadorEmail
{
    public const int TamanhoMaximo = 254;

    private static readonly Regex Padrao = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public static bool EhValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var valor = email.Trim();
        return valor.Length <= TamanhoMaximo && Padrao.IsMatch(valor);
    }
}
