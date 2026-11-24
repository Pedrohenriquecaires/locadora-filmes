namespace LocadoraFilmes.Validation;

/// <summary>Cálculo de idade.</summary>
public static class Idade
{
    public static int Calcular(DateOnly nascimento, DateOnly hoje)
    {
        var idade = hoje.Year - nascimento.Year;

        if (nascimento > hoje.AddYears(-idade))
        {
            idade--;
        }

        return idade;
    }
}
