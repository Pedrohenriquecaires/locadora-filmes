namespace LocadoraFilmes.Validation;

public sealed class ClienteValidator : ValidadorBase<IClienteDados>
{
    public const int IdadeMinima = 18;
    public const int IdadeMaxima = 120;

    private readonly Func<DateOnly> _hoje;

    public ClienteValidator() : this(() => DateOnly.FromDateTime(DateTime.UtcNow))
    {
    }

    public ClienteValidator(Func<DateOnly> hoje)
    {
        _hoje = hoje;
    }

    protected override void Regras(IClienteDados cliente, ResultadoValidacao resultado)
    {
        ValidarTexto(resultado, nameof(IClienteDados.Nome), cliente.Nome, 3, 120);

        if (string.IsNullOrWhiteSpace(cliente.Cpf))
        {
            resultado.Adicionar(nameof(IClienteDados.Cpf), "O campo Cpf é obrigatório.");
        }
        else if (!ValidadorCpf.EhValido(cliente.Cpf))
        {
            resultado.Adicionar(nameof(IClienteDados.Cpf), "CPF inválido.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Email))
        {
            resultado.Adicionar(nameof(IClienteDados.Email), "O campo Email é obrigatório.");
        }
        else if (!ValidadorEmail.EhValido(cliente.Email))
        {
            resultado.Adicionar(nameof(IClienteDados.Email), "E-mail inválido.");
        }

        if (!string.IsNullOrWhiteSpace(cliente.Telefone) && !ValidadorTelefone.EhValido(cliente.Telefone))
        {
            resultado.Adicionar(nameof(IClienteDados.Telefone), "Telefone inválido. Use DDD + número (10 ou 11 dígitos).");
        }

        ValidarNascimento(cliente.DataNascimento, resultado);
    }

    private void ValidarNascimento(DateOnly? nascimento, ResultadoValidacao resultado)
    {
        const string campo = nameof(IClienteDados.DataNascimento);

        if (nascimento is null)
        {
            resultado.Adicionar(campo, "O campo DataNascimento é obrigatório.");
            return;
        }

        var hoje = _hoje();

        if (nascimento.Value > hoje)
        {
            resultado.Adicionar(campo, "A data de nascimento não pode estar no futuro.");
            return;
        }

        var idade = Idade.Calcular(nascimento.Value, hoje);

        if (idade < IdadeMinima)
        {
            resultado.Adicionar(campo, $"O cliente deve ter pelo menos {IdadeMinima} anos.");
        }
        else if (idade > IdadeMaxima)
        {
            resultado.Adicionar(campo, "Data de nascimento inválida.");
        }
    }
}
