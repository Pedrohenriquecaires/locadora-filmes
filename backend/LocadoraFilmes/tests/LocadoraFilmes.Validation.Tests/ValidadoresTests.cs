using LocadoraFilmes.Validation.Tests.Fakes;

namespace LocadoraFilmes.Validation.Tests;

internal static class Assercoes
{
    public static void TemErroNoCampo(ResultadoValidacao resultado, string campo)
    {
        Assert.False(resultado.EhValido);
        Assert.Contains(resultado.Erros, e => e.Campo == campo);
    }
}

public class GeneroValidatorTests
{
    private readonly GeneroValidator _validador = new();

    [Fact]
    public void GeneroValido() => Assert.True(_validador.Validar(new GeneroFake()).EhValido);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void NomeInvalido(string? nome) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new GeneroFake { Nome = nome }), "Nome");

    [Fact]
    public void NomeMuitoLongo() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new GeneroFake { Nome = new string('a', 61) }), "Nome");

    [Fact]
    public void ObjetoNulo_DeveGerarErro()
    {
        var resultado = _validador.Validar(null!);
        Assert.False(resultado.EhValido);
    }
}

public class FilmeValidatorTests
{
    private readonly FilmeValidator _validador = new(() => 2026);

    [Fact]
    public void FilmeValido() => Assert.True(_validador.Validar(new FilmeFake()).EhValido);

    [Fact]
    public void SinopseOpcional() =>
        Assert.True(_validador.Validar(new FilmeFake { Sinopse = null }).EhValido);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TituloObrigatorio(string? titulo) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { Titulo = titulo }), "Titulo");

    [Fact]
    public void TituloMuitoLongo() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { Titulo = new string('x', 151) }), "Titulo");

    [Fact]
    public void SinopseMuitoLonga() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { Sinopse = new string('x', 1001) }), "Sinopse");

    [Theory]
    [InlineData(1500)]
    [InlineData(1887)]
    [InlineData(2028)]
    public void AnoForaDoIntervalo(int ano) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { AnoLancamento = ano }), "AnoLancamento");

    [Theory]
    [InlineData(1888)]
    [InlineData(2027)]
    public void AnoNosLimites_DeveSerValido(int ano) =>
        Assert.True(_validador.Validar(new FilmeFake { AnoLancamento = ano }).EhValido);

    [Theory]
    [InlineData(0)]
    [InlineData(601)]
    public void DuracaoInvalida(int duracao) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { DuracaoMinutos = duracao }), "DuracaoMinutos");

    [Theory]
    [InlineData("L")]
    [InlineData("l")]
    [InlineData("10")]
    [InlineData("12")]
    [InlineData("14")]
    [InlineData("16")]
    [InlineData("18")]
    public void ClassificacaoValida(string classificacao) =>
        Assert.True(_validador.Validar(new FilmeFake { ClassificacaoIndicativa = classificacao }).EhValido);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("20")]
    [InlineData("livre")]
    public void ClassificacaoInvalida(string? classificacao) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { ClassificacaoIndicativa = classificacao }), "ClassificacaoIndicativa");

    [Fact]
    public void PrecoZeroOuNegativoOuAlto_DeveSerInvalido()
    {
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { PrecoDiaria = 0m }), "PrecoDiaria");
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { PrecoDiaria = -1m }), "PrecoDiaria");
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { PrecoDiaria = 1000m }), "PrecoDiaria");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1001)]
    public void QuantidadeInvalida(int quantidade) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { QuantidadeTotal = quantidade }), "QuantidadeTotal");

    [Fact]
    public void QuantidadeZero_DeveSerValida() =>
        Assert.True(_validador.Validar(new FilmeFake { QuantidadeTotal = 0 }).EhValido);

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void GeneroInvalido(int generoId) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new FilmeFake { GeneroId = generoId }), "GeneroId");

    [Fact]
    public void VariosErros_DevemSerAcumulados()
    {
        var resultado = _validador.Validar(new FilmeFake { Titulo = "", DuracaoMinutos = 0, GeneroId = 0 });
        Assert.True(resultado.Erros.Count >= 3);
    }
}

public class ClienteValidatorTests
{
    private readonly ClienteValidator _validador = new(() => new DateOnly(2026, 11, 14));

    [Fact]
    public void ClienteValido() => Assert.True(_validador.Validar(new ClienteFake()).EhValido);

    [Fact]
    public void TelefoneOpcional() =>
        Assert.True(_validador.Validar(new ClienteFake { Telefone = null }).EhValido);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Al")]
    public void NomeInvalido(string? nome) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new ClienteFake { Nome = nome }), "Nome");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("111.111.111-11")]
    [InlineData("123")]
    public void CpfInvalido(string? cpf) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new ClienteFake { Cpf = cpf }), "Cpf");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalido")]
    public void EmailInvalido(string? email) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new ClienteFake { Email = email }), "Email");

    [Fact]
    public void TelefoneInvalido() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new ClienteFake { Telefone = "123" }), "Telefone");

    [Fact]
    public void NascimentoObrigatorio() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new ClienteFake { DataNascimento = null }), "DataNascimento");

    [Fact]
    public void NascimentoNoFuturo() =>
        Assercoes.TemErroNoCampo(
            _validador.Validar(new ClienteFake { DataNascimento = new DateOnly(2030, 1, 1) }), "DataNascimento");

    [Fact]
    public void MenorDeIdade_DeveSerInvalido() =>
        Assercoes.TemErroNoCampo(
            _validador.Validar(new ClienteFake { DataNascimento = new DateOnly(2010, 1, 1) }), "DataNascimento");

    [Fact]
    public void ExatamenteDezoitoAnos_DeveSerValido() =>
        Assert.True(_validador.Validar(new ClienteFake { DataNascimento = new DateOnly(2008, 11, 14) }).EhValido);

    [Fact]
    public void UmDiaAntesDosDezoitoAnos_DeveSerInvalido() =>
        Assercoes.TemErroNoCampo(
            _validador.Validar(new ClienteFake { DataNascimento = new DateOnly(2008, 11, 15) }), "DataNascimento");

    [Fact]
    public void IdadeAbsurda_DeveSerInvalida() =>
        Assercoes.TemErroNoCampo(
            _validador.Validar(new ClienteFake { DataNascimento = new DateOnly(1800, 1, 1) }), "DataNascimento");
}

public class LocacaoValidatorTests
{
    private readonly LocacaoValidator _validador = new();

    [Fact]
    public void LocacaoValida() => Assert.True(_validador.Validar(new LocacaoFake()).EhValido);

    [Fact]
    public void ClienteInvalido() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { ClienteId = 0 }), "ClienteId");

    [Fact]
    public void SemFilmes_DeveSerInvalido()
    {
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { FilmeIds = null }), "FilmeIds");
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { FilmeIds = Array.Empty<int>() }), "FilmeIds");
    }

    [Fact]
    public void FilmesRepetidos_DevemSerInvalidos() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { FilmeIds = new[] { 1, 1 } }), "FilmeIds");

    [Fact]
    public void IdDeFilmeInvalido() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { FilmeIds = new[] { 1, -2 } }), "FilmeIds");

    [Fact]
    public void FilmesDemais_DevemSerInvalidos() =>
        Assercoes.TemErroNoCampo(
            _validador.Validar(new LocacaoFake { FilmeIds = Enumerable.Range(1, 11).ToArray() }), "FilmeIds");

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    [InlineData(-1)]
    public void DiasInvalidos(int dias) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new LocacaoFake { DiasLocacao = dias }), "DiasLocacao");

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    public void DiasNosLimites_DevemSerValidos(int dias) =>
        Assert.True(_validador.Validar(new LocacaoFake { DiasLocacao = dias }).EhValido);
}

public class DiasLocacaoValidatorTests
{
    private readonly DiasLocacaoValidator _validador = new();

    [Theory]
    [InlineData(1, true)]
    [InlineData(30, true)]
    [InlineData(0, false)]
    [InlineData(31, false)]
    public void ValidaPrazo(int dias, bool esperado) =>
        Assert.Equal(esperado, _validador.Validar(new DiasFake(dias)).EhValido);
}

public class UsuarioValidatorTests
{
    private readonly UsuarioValidator _validador = new();

    [Fact]
    public void UsuarioValido() => Assert.True(_validador.Validar(new UsuarioFake()).EhValido);

    [Theory]
    [InlineData("Admin")]
    [InlineData("atendente")]
    public void PerfilValido_IgnorandoCaixa(string perfil) =>
        Assert.True(_validador.Validar(new UsuarioFake { Perfil = perfil }).EhValido);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Gerente")]
    public void PerfilInvalido(string? perfil) =>
        Assercoes.TemErroNoCampo(_validador.Validar(new UsuarioFake { Perfil = perfil }), "Perfil");

    [Fact]
    public void SenhaFraca() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new UsuarioFake { Senha = "123" }), "Senha");

    [Fact]
    public void EmailInvalido() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new UsuarioFake { Email = "xyz" }), "Email");

    [Fact]
    public void NomeCurto() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new UsuarioFake { Nome = "Jo" }), "Nome");
}

public class LoginValidatorTests
{
    private readonly LoginValidator _validador = new();

    [Fact]
    public void LoginValido() => Assert.True(_validador.Validar(new LoginFake()).EhValido);

    [Fact]
    public void EmailEObrigatorioEValido()
    {
        Assercoes.TemErroNoCampo(_validador.Validar(new LoginFake { Email = null }), "Email");
        Assercoes.TemErroNoCampo(_validador.Validar(new LoginFake { Email = "abc" }), "Email");
    }

    [Fact]
    public void SenhaObrigatoria() =>
        Assercoes.TemErroNoCampo(_validador.Validar(new LoginFake { Senha = "" }), "Senha");
}
