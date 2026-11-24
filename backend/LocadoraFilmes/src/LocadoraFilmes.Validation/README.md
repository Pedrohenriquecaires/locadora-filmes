# LocadoraFilmes.Validation

Biblioteca de validação do sistema **Locadora de Filmes**.

```csharp
// Registro na injeção de dependência
services.AddLocadoraValidation();

// Uso
var resultado = validador.Validar(dados);
if (!resultado.EhValido) { /* resultado.Erros */ }

// ou lançando exceção
resultado.LancarSeInvalido();
```

Inclui: `ValidadorCpf`, `ValidadorEmail`, `ValidadorTelefone`, `RegraSenha`, `Idade` e validadores para
gênero, filme, cliente, locação, usuário e login. Os validadores trabalham sobre interfaces
(`IFilmeDados`, `IClienteDados` ...), portanto qualquer DTO que as implemente pode ser validado.
