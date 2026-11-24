using LocadoraFilmes.Validation;

namespace Microsoft.Extensions.DependencyInjection;

public static class LocadoraValidationServiceCollectionExtensions
{
    /// <summary>Registra todos os validadores da biblioteca na injeção de dependência.</summary>
    public static IServiceCollection AddLocadoraValidation(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<IGeneroDados>>(_ => new GeneroValidator());
        services.AddSingleton<IValidator<IFilmeDados>>(_ => new FilmeValidator());
        services.AddSingleton<IValidator<IClienteDados>>(_ => new ClienteValidator());
        services.AddSingleton<IValidator<ILocacaoDados>>(_ => new LocacaoValidator());
        services.AddSingleton<IValidator<IDiasLocacaoDados>>(_ => new DiasLocacaoValidator());
        services.AddSingleton<IValidator<IUsuarioDados>>(_ => new UsuarioValidator());
        services.AddSingleton<IValidator<ILoginDados>>(_ => new LoginValidator());

        return services;
    }
}
