namespace LocadoraFilmes.Validation;

public sealed class UsuarioValidator : ValidadorBase<IUsuarioDados>
{
    public static readonly IReadOnlyCollection<string> PerfisPermitidos = new[] { "Admin", "Atendente" };

    protected override void Regras(IUsuarioDados usuario, ResultadoValidacao resultado)
    {
        ValidarTexto(resultado, nameof(IUsuarioDados.Nome), usuario.Nome, 3, 120);

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            resultado.Adicionar(nameof(IUsuarioDados.Email), "O campo Email é obrigatório.");
        }
        else if (!ValidadorEmail.EhValido(usuario.Email))
        {
            resultado.Adicionar(nameof(IUsuarioDados.Email), "E-mail inválido.");
        }

        foreach (var problema in RegraSenha.Verificar(usuario.Senha))
        {
            resultado.Adicionar(nameof(IUsuarioDados.Senha), problema);
        }

        var perfil = usuario.Perfil?.Trim();
        if (string.IsNullOrEmpty(perfil)
            || !PerfisPermitidos.Contains(perfil, StringComparer.OrdinalIgnoreCase))
        {
            resultado.Adicionar(nameof(IUsuarioDados.Perfil),
                $"O perfil deve ser um dos valores: {string.Join(", ", PerfisPermitidos)}.");
        }
    }
}

public sealed class LoginValidator : ValidadorBase<ILoginDados>
{
    protected override void Regras(ILoginDados login, ResultadoValidacao resultado)
    {
        if (string.IsNullOrWhiteSpace(login.Email))
        {
            resultado.Adicionar(nameof(ILoginDados.Email), "O campo Email é obrigatório.");
        }
        else if (!ValidadorEmail.EhValido(login.Email))
        {
            resultado.Adicionar(nameof(ILoginDados.Email), "E-mail inválido.");
        }

        if (string.IsNullOrEmpty(login.Senha))
        {
            resultado.Adicionar(nameof(ILoginDados.Senha), "O campo Senha é obrigatório.");
        }
    }
}
