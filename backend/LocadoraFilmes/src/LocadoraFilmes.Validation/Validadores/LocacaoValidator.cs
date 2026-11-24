namespace LocadoraFilmes.Validation;

/// <summary>Valida o prazo (em dias) de uma locação.</summary>
public sealed class DiasLocacaoValidator : ValidadorBase<IDiasLocacaoDados>
{
    public const int MinimoDias = 1;
    public const int MaximoDias = 30;

    protected override void Regras(IDiasLocacaoDados dados, ResultadoValidacao resultado) =>
        ValidarDias(resultado, dados.DiasLocacao);

    internal static void ValidarDias(ResultadoValidacao resultado, int dias)
    {
        if (dias < MinimoDias || dias > MaximoDias)
        {
            resultado.Adicionar(nameof(IDiasLocacaoDados.DiasLocacao),
                $"O prazo da locação deve estar entre {MinimoDias} e {MaximoDias} dias.");
        }
    }
}

/// <summary>Valida os dados de criação de uma locação.</summary>
public sealed class LocacaoValidator : ValidadorBase<ILocacaoDados>
{
    public const int MaximoFilmesPorLocacao = 10;

    protected override void Regras(ILocacaoDados locacao, ResultadoValidacao resultado)
    {
        if (locacao.ClienteId <= 0)
        {
            resultado.Adicionar(nameof(ILocacaoDados.ClienteId), "Informe um cliente válido.");
        }

        var ids = locacao.FilmeIds;
        const string campo = nameof(ILocacaoDados.FilmeIds);

        if (ids is null || ids.Count == 0)
        {
            resultado.Adicionar(campo, "Informe ao menos um filme.");
        }
        else
        {
            if (ids.Count > MaximoFilmesPorLocacao)
            {
                resultado.Adicionar(campo, $"Uma locação pode ter no máximo {MaximoFilmesPorLocacao} filmes.");
            }

            if (ids.Any(id => id <= 0))
            {
                resultado.Adicionar(campo, "A lista contém identificadores de filme inválidos.");
            }

            if (ids.Distinct().Count() != ids.Count)
            {
                resultado.Adicionar(campo, "A lista de filmes não pode conter itens repetidos.");
            }
        }

        DiasLocacaoValidator.ValidarDias(resultado, locacao.DiasLocacao);
    }
}
