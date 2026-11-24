namespace LocadoraFilmes.Validation.Tests;

public class ValidadorCpfTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData("111.444.777-35")]
    [InlineData("168.995.350-09")]
    public void CpfValido_DeveRetornarVerdadeiro(string cpf) =>
        Assert.True(ValidadorCpf.EhValido(cpf));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("111.111.111-11")]
    [InlineData("529.982.247-26")]
    [InlineData("529.982.247-250")]
    [InlineData("abc.def.ghi-jk")]
    public void CpfInvalido_DeveRetornarFalso(string? cpf) =>
        Assert.False(ValidadorCpf.EhValido(cpf));

    [Fact]
    public void Normalizar_DeveManterSomenteDigitos()
    {
        Assert.Equal("52998224725", ValidadorCpf.Normalizar("529.982.247-25"));
        Assert.Equal(string.Empty, ValidadorCpf.Normalizar(null));
    }
}

public class ValidadorEmailTests
{
    [Theory]
    [InlineData("ana@email.com")]
    [InlineData("  ana.souza+tag@dominio.com.br  ")]
    public void EmailValido(string email) => Assert.True(ValidadorEmail.EhValido(email));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("semarroba.com")]
    [InlineData("a@b")]
    [InlineData("a b@c.com")]
    [InlineData("@dominio.com")]
    public void EmailInvalido(string? email) => Assert.False(ValidadorEmail.EhValido(email));

    [Fact]
    public void EmailMuitoLongo_DeveSerInvalido()
    {
        var email = new string('a', 250) + "@b.com";
        Assert.False(ValidadorEmail.EhValido(email));
    }
}

public class ValidadorTelefoneTests
{
    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("1187654321")]
    [InlineData("(21) 3456-7890")]
    public void TelefoneValido(string telefone) => Assert.True(ValidadorTelefone.EhValido(telefone));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("00987654321")]
    [InlineData("11887654321")] // celular sem o 9
    [InlineData("119876543210")]
    public void TelefoneInvalido(string? telefone) => Assert.False(ValidadorTelefone.EhValido(telefone));
}

public class RegraSenhaTests
{
    [Theory]
    [InlineData("Admin@123")]
    [InlineData("Abcdefg1")]
    public void SenhaValida(string senha) => Assert.True(RegraSenha.EhValida(senha));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Ab1")]
    [InlineData("semmaiuscula1")]
    [InlineData("SEMMINUSCULA1")]
    [InlineData("SemNumeroAqui")]
    public void SenhaInvalida(string? senha) => Assert.False(RegraSenha.EhValida(senha));

    [Fact]
    public void SenhaSemRequisitos_DeveListarTodosOsProblemas()
    {
        var problemas = RegraSenha.Verificar("abc");
        Assert.Equal(3, problemas.Count); // tamanho, maiúscula e número
    }

    [Fact]
    public void SenhaMuitoLonga_DeveSerInvalida() =>
        Assert.False(RegraSenha.EhValida("Aa1" + new string('x', RegraSenha.TamanhoMaximo)));
}

public class IdadeTests
{
    [Theory]
    [InlineData(2000, 11, 14, 2026, 11, 14, 26)] // aniversário hoje
    [InlineData(2000, 11, 15, 2026, 11, 14, 25)] // aniversário amanhã
    [InlineData(2000, 1, 1, 2026, 11, 14, 26)]
    [InlineData(2008, 2, 29, 2026, 2, 28, 17)]   // ano bissexto
    public void Calcular_DeveConsiderarMesEDia(int ny, int nm, int nd, int hy, int hm, int hd, int esperado) =>
        Assert.Equal(esperado, Idade.Calcular(new DateOnly(ny, nm, nd), new DateOnly(hy, hm, hd)));
}

public class ResultadoValidacaoTests
{
    [Fact]
    public void NovoResultado_DeveSerValido()
    {
        var resultado = new ResultadoValidacao();
        Assert.True(resultado.EhValido);
        Assert.Empty(resultado.Erros);
    }

    [Fact]
    public void Adicionar_DeveTornarInvalidoEAgruparPorCampo()
    {
        var resultado = new ResultadoValidacao()
            .Adicionar("Nome", "erro 1")
            .Adicionar("Nome", "erro 2")
            .Adicionar("Email", "erro 3");

        Assert.False(resultado.EhValido);
        var dicionario = resultado.ParaDicionario();
        Assert.Equal(2, dicionario["Nome"].Length);
        Assert.Single(dicionario["Email"]);
    }

    [Fact]
    public void LancarSeInvalido_DeveLancarExcecaoComOsErros()
    {
        var resultado = new ResultadoValidacao().Adicionar("Nome", "erro");
        var ex = Assert.Throws<ValidacaoException>(resultado.LancarSeInvalido);
        Assert.Single(ex.Resultado.Erros);
    }

    [Fact]
    public void LancarSeInvalido_NaoDeveLancarQuandoValido() =>
        new ResultadoValidacao().LancarSeInvalido();

    [Fact]
    public void ExcecaoComCampoEMensagem_DeveConterOErro()
    {
        var ex = new ValidacaoException("Campo", "mensagem");
        Assert.Equal("Campo", ex.Resultado.Erros[0].Campo);
    }
}

public class ValidadorDiretorTests
{
    [Theory]
    [InlineData("Fernando Meirelles")]
    [InlineData("Jean-Luc Godard")]
    [InlineData("M. Night Shyamalan")]
    [InlineData("Kátia Lund")]
    [InlineData(null)]
    [InlineData("")]
    public void DiretorValido_DeveRetornarVerdadeiro(string? nome) =>
        Assert.True(ValidadorDiretor.EhValido(nome));

    [Theory]
    [InlineData("Jo")]
    [InlineData("12345")]
    [InlineData("Diretor@2024")]
    [InlineData("...")]
    public void DiretorInvalido_DeveRetornarFalso(string nome) =>
        Assert.False(ValidadorDiretor.EhValido(nome));

    [Fact]
    public void DiretorComMaisDe120Caracteres_DeveSerInvalido()
    {
        // Arrange
        var nome = new string('a', 121);

        // Act
        var resultado = ValidadorDiretor.EhValido(nome);

        // Assert
        Assert.False(resultado);
    }
}