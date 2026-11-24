namespace LocadoraFilmes.Validation;

/// <summary>Validação e normalização de CPF.</summary>
public static class ValidadorCpf
{
    /// <summary>Remove tudo que não for dígito.</summary>
    public static string Normalizar(string? cpf) =>
        new string((cpf ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    /// <summary>Verifica tamanho, sequências repetidas e dígitos verificadores.</summary>
    public static bool EhValido(string? cpf)
    {
        var digitos = Normalizar(cpf);

        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
        {
            return false;
        }

        return CalcularDigito(digitos, 9) == digitos[9] - '0'
            && CalcularDigito(digitos, 10) == digitos[10] - '0';
    }

    private static int CalcularDigito(string digitos, int quantidade)
    {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
        {
            soma += (digitos[i] - '0') * (quantidade + 1 - i);
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
