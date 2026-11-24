namespace LocadoraFilmes.Validation;

/// <summary>Validação de telefone brasileiro (fixo com 10 dígitos ou celular com 11).</summary>
public static class ValidadorTelefone
{
    public static string Normalizar(string? telefone) =>
        new string((telefone ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    public static bool EhValido(string? telefone)
    {
        var digitos = Normalizar(telefone);

        if (digitos.Length is not (10 or 11))
        {
            return false;
        }

        var ddd = int.Parse(digitos.AsSpan(0, 2));
        if (ddd < 11)
        {
            return false;
        }

        // Celulares (11 dígitos) começam com 9 após o DDD.
        return digitos.Length == 10 || digitos[2] == '9';
    }
}
