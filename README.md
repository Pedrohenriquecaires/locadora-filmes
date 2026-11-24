# 🎬 Locadora de Filmes — Sistema Full-Stack

Sistema de gestão de uma locadora de filmes: cadastro de gêneros, filmes e clientes, controle de locações com estoque, devolução com multa por atraso e painel gerencial.

**Stack:** .NET 8 · ASP.NET Core Web API · Entity Framework Core + SQLite · JWT · xUnit · NuGet (feed local) · Angular (standalone)

## 👥 Integrantes

- PEDRO CAIRES
- GABRIEL FLORENCIO
- LUCAS BOSCARDIN
- ABIMAEL JABEZ
- EDUARDO SCARPINELLI
- ADRIAN MELLO
- JULIANO ALBUQUERQUE

## 📖 Cenário

A locadora precisa controlar quais filmes possui (e quantas cópias de cada), quem são seus clientes e quais filmes estão alugados com cada cliente. O atendente registra a locação de um ou mais filmes para um cliente por um número de dias; na devolução o sistema calcula automaticamente a multa caso o prazo tenha sido ultrapassado. O gerente acompanha os resultados em um painel (dashboard).

### Regras de negócio (Services)

| Regra | Onde |
|-------|------|
| Cada filme tem estoque (`QuantidadeTotal` / `QuantidadeDisponivel`): a locação baixa o estoque e a devolução repõe | `LocacaoService` |
| Valor da locação = soma das diárias dos filmes × número de dias (1 a 30) | `LocacaoService` |
| Multa por atraso = 50% da soma das diárias por dia de atraso | `LocacaoService` |
| Cliente com locação em atraso não pode fazer nova locação | `LocacaoService` |
| Cliente pode ter no máximo 3 locações ativas ao mesmo tempo | `LocacaoService` |
| Não é possível excluir gênero com filmes, nem filme/cliente com locações | `GeneroService`, `FilmeService`, `ClienteService` |
| CPF e e-mail de cliente não podem se repetir | `ClienteService` |
| Relatório: totais, locações ativas/atrasadas, receita, top 5 filmes, ranking de clientes e aluguéis por mês | `RelatorioService` |

### Regras de validação (biblioteca `LocadoraFilmes.Validation`)

| Entidade | Regras |
|----------|--------|
| **Cliente** | Nome obrigatório (3–120 caracteres) · CPF válido (dígitos verificadores) · e-mail válido · telefone BR (DDD + 10/11 dígitos) · maior de 18 anos e data de nascimento não futura |
| **Filme** | Título obrigatório (1–150) · sinopse até 1000 · ano entre 1888 e o ano que vem · duração 1–600 min · classificação indicativa L/10/12/14/16/18 · diária > 0 e ≤ 999,99 · quantidade 0–1000 · gênero obrigatório |
| **Filme – Diretor** *(v1.1.0)* | Opcional; quando informado: 3–120 caracteres, apenas letras, espaço, ponto, apóstrofo ou hífen |
| **Locação** | Cliente obrigatório · 1 a 10 filmes, sem repetição · 1 a 30 dias |
| **Gênero** | Nome obrigatório (2–60) |
| **Usuário / Login** | Nome (3–120) · e-mail válido · política de senha · perfil `Admin` ou `Atendente` |

## 🗂️ Mapeamento das entidades

| Entidade | Atributos | Relacionamentos |
|----------|-----------|-----------------|
| **Genero** | Id, Nome | 1:N com Filme |
| **Filme** | Id, Titulo, Sinopse, Diretor, AnoLancamento, DuracaoMinutos, ClassificacaoIndicativa, PrecoDiaria, QuantidadeTotal, QuantidadeDisponivel, CriadoEm, GeneroId | N:1 com Genero · 1:N com LocacaoItem |
| **Cliente** | Id, Nome, Cpf, Email, Telefone, DataNascimento, CriadoEm | 1:N com Locacao |
| **Locacao** | Id, ClienteId, DataLocacao, DataPrevistaDevolucao, DataDevolucao, ValorTotal, ValorMulta, Status (Ativa/Devolvida) | N:1 com Cliente · 1:N com LocacaoItem |
| **LocacaoItem** | Id, LocacaoId, FilmeId, PrecoDiaria (preço no momento da locação) | N:1 com Locacao · N:1 com Filme |
| **Usuario** | Id, Nome, Email, SenhaHash, Perfil (Admin/Atendente), CriadoEm | — (usado na autenticação) |

Diagrama completo em [`docs/arquitetura.md`](docs/arquitetura.md).

## ✅ Onde cada requisito foi aplicado

Caminhos do back-end relativos a `backend/LocadoraFilmes/`.

| # | Requisito | Projeto / arquivo | Classe / método |
|---|-----------|-------------------|-----------------|
| R1 | Class Library | `src/LocadoraFilmes.Validation/` (sem dependência de ASP.NET Core ou EF Core) | `ClienteValidator`, `FilmeValidator`, `LocacaoValidator`, `GeneroValidator`, `UsuarioValidator`, `LoginValidator`; regras em `Regras/` (`ValidadorCpf`, `ValidadorEmail`, `ValidadorTelefone`, `RegraSenha`, `Idade`, `ValidadorDiretor`) |
| R2 | Testes xUnit | `tests/LocadoraFilmes.Validation.Tests/` | `RegrasTests.cs` (ex.: `ValidadorCpfTests` com `[Theory]` + `[InlineData]`), `ValidadoresTests.cs`, `InjecaoDependenciaTests.cs`; padrão Arrange-Act-Assert (ex.: `DiretorComMaisDe120Caracteres_DeveSerInvalido`) |
| R3 | NuGet + SemVer | `NuGet.config`, `nuget-local/` (versões **1.0.0** e **1.1.0**), `scripts/publicar-pacote.ps1/.sh` | `LocadoraFilmes.Api.csproj` → `<PackageReference Include="LocadoraFilmes.Validation" Version="1.1.0" />` |
| R4 | Web API + DI | `src/LocadoraFilmes.Api/Program.cs`, `Controllers/`, `Services/` | `AddScoped<IFilmeService, FilmeService>()` etc. e `AddLocadoraValidation()`; services recebidos pelo construtor dos controllers |
| R5 | EF Core Code First + Migrations | `Data/LocadoraDbContext.cs`, `Data/Configurations/`, `Data/Migrations/` | Migrations `Inicial` e `AdicionaDiretorFilme`; relacionamentos 1:N Genero→Filme, Cliente→Locacao, Locacao→LocacaoItem |
| R6 | CRUD + status HTTP | `Controllers/FilmesController.cs`, `ClientesController.cs`, `GenerosController.cs`, `LocacoesController.cs`; `Handlers/ApiExceptionHandler.cs` | `Criar` → 201 com `CreatedAtAction` (Location) · `Remover` → 204 · `ValidacaoException` → 400 com mensagens da biblioteca · `RecursoNaoEncontradoException` → 404 |
| R7 | JWT | `Controllers/AuthController.cs`, `Services/TokenService.cs`, `Program.cs`, `appsettings.json` | `AuthController.Login` · `TokenService.Gerar` · `[Authorize]` + políticas `AdminOuAtendente` / `ApenasAdmin` · `Jwt:ExpiracaoMinutos = 60` |
| R8 | Angular standalone | `frontend/src/app/paginas/`, `frontend/src/app/servicos/` | `ClientesLista`, `ClienteForm`, `FilmesLista`, `FilmeForm` (listar/cadastrar/editar/excluir) · `ClienteDetalhes` (locações do cliente = relacionamento) · `Dashboard` (relatório) · services com `HttpClient` |
| R9 | Integração front ↔ back | `frontend/src/app/interceptors/auth-interceptor.ts`, `guardas/auth-guard.ts`, `paginas/login/`, `utils/erros.ts`; CORS em `Program.cs` | `authInterceptor` (anexa o token) · `authGuard` · `mensagensDeErro` (exibe erros 400) · política `PoliticaCors` (origem `http://localhost:4200`) |
| — | Relatório / cálculo | `Controllers/RelatoriosController.cs`, `Services/RelatorioService.cs` → `frontend/.../servicos/dashboard.ts` | `GET /api/relatorios/dashboard` · `RelatorioService.ObterDashboardAsync` |
| — | Seed | `Data/DbSeeder.cs` | `DbSeeder.SeedAsync`: 2 usuários, 6 gêneros, 10 filmes, 3 clientes, 2 locações |

### Evolução da biblioteca (SemVer)

| Versão | Mudança | Tipo |
|--------|---------|------|
| 1.0.0 | Versão inicial: validadores de Cliente, Filme, Locação, Gênero, Usuário e Login | — |
| 1.1.0 | Nova regra `ValidadorDiretor` (nome do diretor do filme) | **MINOR**: adiciona funcionalidade sem quebrar quem usa a 1.0.0 (nenhuma API pública foi removida ou alterada) |

## ▶️ Como executar do zero

**Pré-requisitos:** .NET SDK 8+, Node.js 20+ e a ferramenta do EF:

```bash
dotnet tool install --global dotnet-ef
```

**1. Testes da biblioteca**

```bash
cd backend/LocadoraFilmes
dotnet test
```

**2. API** (http://localhost:5080)

```bash
cd backend/LocadoraFilmes
dotnet ef database update -p src/LocadoraFilmes.Api
dotnet run --project src/LocadoraFilmes.Api
```

A API consome a biblioteca pelo pacote `LocadoraFilmes.Validation 1.1.0` do feed local `nuget-local/` (configurado no `NuGet.config`). Na inicialização ela aplica as migrations e carrega o seed. O banco fica em `src/LocadoraFilmes.Api/App_Data/locadora.db`, então os dados sobrevivem a um reinício.

**3. Front-end** (http://localhost:4200)

```bash
cd frontend
npm install
npx ng serve
```

**(Opcional) Publicar uma nova versão da biblioteca no feed local**

```powershell
./scripts/publicar-pacote.ps1 -Versao 1.2.0      # Linux/Mac: ./scripts/publicar-pacote.sh 1.2.0
```

## 🔑 Usuário padrão

| Perfil | E-mail | Senha |
|--------|--------|-------|
| Admin | `admin@locadora.com` | `Admin@123` |
| Atendente | `atendente@locadora.com` | `Atendente@123` |

O Atendente pode cadastrar e editar; somente o Admin pode excluir.

## 🌐 Endpoints principais

| Recurso | Rotas |
|---------|-------|
| Auth | `POST /api/auth/login` · `POST /api/auth/registrar` · `GET /api/auth/eu` |
| Gêneros | `GET/POST /api/generos` · `GET/PUT/DELETE /api/generos/{id}` |
| Filmes | `GET/POST /api/filmes` · `GET/PUT/DELETE /api/filmes/{id}` |
| Clientes | `GET/POST /api/clientes` · `GET/PUT/DELETE /api/clientes/{id}` |
| Locações | `GET/POST /api/locacoes` · `GET/PUT/DELETE /api/locacoes/{id}` · `POST /api/locacoes/{id}/devolucao` |
| Relatório | `GET /api/relatorios/dashboard` |

Exemplos de chamadas em `backend/LocadoraFilmes/requests.http`.
