namespace LocadoraFilmes.Validation;

/// <summary>Política de senha: 8 a 128 caracteres com maiúscula, minúscula e número.</summary>
public static class RegraSenha
{
    public const int TamanhoMinimo = 8;
    public const int TamanhoMaximo = 128;

    /// <summary>Retorna as mensagens das regras violadas (lista vazia = senha válida).</summary>
    public static IReadOnlyList<string> Verificar(string? senha)
    {
        var problemas = new List<string>();

        if (string.IsNullOrEmpty(senha))
        {
            problemas.Add("O campo Senha é obrigatório.");
            return problemas;
        }

        if (senha.Length < TamanhoMinimo)
        {
            problemas.Add($"A senha deve ter no mínimo {TamanhoMinimo} caracteres.");
        }

        if (senha.Length > TamanhoMaximo)
        {
            problemas.Add($"A senha deve ter no máximo {TamanhoMaximo} caracteres.");
        }

        if (!senha.Any(char.IsUpper))
        {
            problemas.Add("A senha deve conter ao menos uma letra maiúscula.");
        }

        if (!senha.Any(char.IsLower))
        {
            problemas.Add("A senha deve conter ao menos uma letra minúscula.");
        }

        if (!senha.Any(char.IsDigit))
        {
            problemas.Add("A senha deve conter ao menos um número.");
        }

        return problemas;
    }

    public static bool EhValida(string? senha) => Verificar(senha).Count == 0;
}
