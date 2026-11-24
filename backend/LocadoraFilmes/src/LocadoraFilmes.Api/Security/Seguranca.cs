using System.Security.Cryptography;

namespace LocadoraFilmes.Api.Security;

public static class Perfis
{
    public const string Admin = "Admin";
    public const string Atendente = "Atendente";

    /// <summary>Retorna o nome canônico do perfil (ignorando maiúsculas/minúsculas) ou null.</summary>
    public static string? Normalizar(string? perfil)
    {
        if (string.Equals(perfil?.Trim(), Admin, StringComparison.OrdinalIgnoreCase))
        {
            return Admin;
        }

        if (string.Equals(perfil?.Trim(), Atendente, StringComparison.OrdinalIgnoreCase))
        {
            return Atendente;
        }

        return null;
    }
}

public static class Politicas
{
    public const string AdminOuAtendente = "AdminOuAtendente";
    public const string ApenasAdmin = "ApenasAdmin";
}

public class JwtSettings
{
    public const string Secao = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 60;
}

public interface ISenhaHasher
{
    string Gerar(string senha);
    bool Verificar(string senha, string hashArmazenado);
}

/// <summary>Hash de senha com PBKDF2 (SHA-256) e salt aleatório. Formato: v1.iteracoes.salt.hash</summary>
public sealed class SenhaHasher : ISenhaHasher
{
    private const int Iteracoes = 100_000;
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;

    public string Gerar(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);

        return $"v1.{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string hashArmazenado)
    {
        var partes = hashArmazenado.Split('.');

        if (partes.Length != 4 || partes[0] != "v1" || !int.TryParse(partes[1], out var iteracoes))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var calculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);

            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
