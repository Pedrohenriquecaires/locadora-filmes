using LocadoraFilmes.Validation.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace LocadoraFilmes.Validation.Tests;

public class InjecaoDependenciaTests
{
    [Fact]
    public void AddLocadoraValidation_DeveRegistrarTodosOsValidadores()
    {
        var provider = new ServiceCollection().AddLocadoraValidation().BuildServiceProvider();

        Assert.IsType<GeneroValidator>(provider.GetRequiredService<IValidator<IGeneroDados>>());
        Assert.IsType<FilmeValidator>(provider.GetRequiredService<IValidator<IFilmeDados>>());
        Assert.IsType<ClienteValidator>(provider.GetRequiredService<IValidator<IClienteDados>>());
        Assert.IsType<LocacaoValidator>(provider.GetRequiredService<IValidator<ILocacaoDados>>());
        Assert.IsType<DiasLocacaoValidator>(provider.GetRequiredService<IValidator<IDiasLocacaoDados>>());
        Assert.IsType<UsuarioValidator>(provider.GetRequiredService<IValidator<IUsuarioDados>>());
        Assert.IsType<LoginValidator>(provider.GetRequiredService<IValidator<ILoginDados>>());
    }

    [Fact]
    public void ValidadorResolvido_DeveFuncionar()
    {
        var provider = new ServiceCollection().AddLocadoraValidation().BuildServiceProvider();
        var validador = provider.GetRequiredService<IValidator<IClienteDados>>();

        Assert.True(validador.Validar(new ClienteFake()).EhValido);
        Assert.False(validador.Validar(new ClienteFake { Cpf = "000" }).EhValido);
    }
}
