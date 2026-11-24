# Locadora de Filmes — Back-end (.NET 8)

Web API REST (somente back-end, sem front-end) para um sistema de locadora de filmes.
**Stack:** ASP.NET Core Web API · Entity Framework Core + SQLite · JWT · xUnit · NuGet (feed local).

## Estrutura

```
LocadoraFilmes/
├── LocadoraFilmes.sln
├── NuGet.config                  # nuget.org + feed local ./nuget-local
├── nuget-local/                  # feed NuGet local (pacotes publicados aqui)
├── scripts/                      # publicar-pacote / executar-api (.sh e .ps1)
├── requests.http                 # exemplos de chamadas
├── src/
│   ├── LocadoraFilmes.Validation/   # Class Library de validação (vira pacote NuGet)
│   └── LocadoraFilmes.Api/          # Web API
│       ├── Controllers/  Services/  Dtos/  Domain/  Security/  Handlers/  Exceptions/
│       └── Data/ (DbContext, Configurations, Migrations, DbSeeder)
└── tests/
    └── LocadoraFilmes.Validation.Tests/   # testes xUnit
```

## Pré-requisitos

- .NET SDK 8.0 ou superior (o projeto tem `RollForward=LatestMajor`, então roda com SDK/runtime 9 ou 10).
- Acesso ao nuget.org no primeiro `restore`.

## Executando

```bash
dotnet run --project src/LocadoraFilmes.Api        # http://localhost:5080
```

Na inicialização a API aplica as **migrations** (cria `App_Data/locadora.db`) e executa o **seed**.

Usuários criados pelo seed (somente desenvolvimento):

| Perfil    | E-mail                   | Senha          |
|-----------|--------------------------|----------------|
| Admin     | admin@locadora.com       | Admin@123      |
| Atendente | atendente@locadora.com   | Atendente@123  |

O seed também cria 6 gêneros, 10 filmes, 3 clientes e 2 locações (uma devolvida, uma ativa).

## Autenticação e autorização

1. `POST /api/auth/login` → retorna o token JWT.
2. Envie `Authorization: Bearer <token>` nas demais chamadas.

| Operação                              | Quem pode                |
|---------------------------------------|--------------------------|
| `GET` (listar/consultar)              | qualquer usuário autenticado |
| `POST` / `PUT` / devolução            | Admin ou Atendente       |
| `DELETE` e `POST /api/auth/registrar` | somente Admin            |

Há uma *fallback policy*: todo endpoint exige autenticação, exceto os marcados com `[AllowAnonymous]`
(`/api/auth/login`, `/` e `/health`).

Configuração em `appsettings.json` (`Jwt`, `Cors`, `ConnectionStrings`, `Seed`).
**Em produção** troque `Jwt:Key` (variável de ambiente `Jwt__Key`) e as origens CORS (`Cors__Origens__0`...).

## Endpoints

| Recurso   | Rotas |
|-----------|-------|
| Auth      | `POST /api/auth/login` · `POST /api/auth/registrar` · `GET /api/auth/eu` |
| Gêneros   | `GET/POST /api/generos` · `GET/PUT/DELETE /api/generos/{id}` |
| Filmes    | `GET/POST /api/filmes` (`?titulo=&generoId=`) · `GET/PUT/DELETE /api/filmes/{id}` |
| Clientes  | `GET/POST /api/clientes` (`?nome=`) · `GET/PUT/DELETE /api/clientes/{id}` |
| Locações  | `GET/POST /api/locacoes` (`?clienteId=&status=`) · `GET/PUT/DELETE /api/locacoes/{id}` · `POST /api/locacoes/{id}/devolucao` |

### Códigos HTTP

`200` OK · `201` Created (com `Location`) · `204` No Content (DELETE) · `400` validação / JSON inválido ·
`401` não autenticado / credenciais inválidas · `403` sem permissão · `404` não encontrado ·
`409` conflito (duplicidade, registro em uso, sem estoque) · `422` regra de negócio · `500` erro inesperado.
Os erros seguem o padrão `ProblemDetails` (RFC 7807); erros de validação trazem a lista `errors` por campo.

### Regras de negócio das locações

- Cada filme tem estoque (`QuantidadeTotal` / `QuantidadeDisponivel`); a locação baixa o estoque e a devolução repõe.
- Valor = soma das diárias × dias (1 a 30). Multa por atraso = 50% da soma das diárias por dia de atraso.
- Cliente com locação em atraso, ou com 3 locações ativas, não pode alugar (422).
- Não é possível excluir gênero com filmes, nem filme/cliente com locações (409).

## Biblioteca de validação (`LocadoraFilmes.Validation`)

Class Library independente: define interfaces de dados (`IFilmeDados`, `IClienteDados`, ...) e validadores
(`FilmeValidator`, `ClienteValidator`, `LocacaoValidator`, `UsuarioValidator`, `LoginValidator`, `GeneroValidator`)
com regras reutilizáveis (CPF com dígitos verificadores, e-mail, telefone BR, política de senha, idade).
Os DTOs da API implementam essas interfaces; os *services* chamam `validador.Validar(dto).LancarSeInvalido()`
e o `ApiExceptionHandler` converte a `ValidacaoException` em `400` com os erros por campo.
Registro na DI: `services.AddLocadoraValidation()`.

### Testes unitários (xUnit)

```bash
dotnet test
```

### Pacotes NuGet e feed local

`dotnet pack` gera `LocadoraFilmes.Validation.<versao>.nupkg` e `.snupkg` (símbolos). O `NuGet.config` já aponta para `./nuget-local`.

```bash
./scripts/publicar-pacote.sh 1.0.0      # Windows: ./scripts/publicar-pacote.ps1 -Versao 1.0.0
./scripts/executar-api.sh --pacote 1.0.0  # API consumindo o PACOTE do feed local
```

O `publicar-pacote` executa os testes, empacota, publica no feed (`dotnet nuget push --source ./nuget-local`)
e limpa o cache. A API usa `ProjectReference` por padrão e passa a usar `PackageReference` (pacote do feed local)
com `-p:UsarPacoteValidacao=true`.

Comandos manuais equivalentes:

```bash
dotnet pack src/LocadoraFilmes.Validation -c Release -o artifacts/nupkg
dotnet nuget push artifacts/nupkg/*.nupkg --source ./nuget-local
dotnet restore src/LocadoraFilmes.Api -p:UsarPacoteValidacao=true
dotnet build   src/LocadoraFilmes.Api -p:UsarPacoteValidacao=true --no-restore
```

## Banco de dados e migrations

Entidades: `Usuario`, `Genero`, `Filme`, `Cliente`, `Locacao`, `LocacaoItem`
(Genero 1—N Filme · Cliente 1—N Locacao · Locacao 1—N LocacaoItem N—1 Filme).

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add NomeDaMigration -p src/LocadoraFilmes.Api -o Data/Migrations
dotnet ef database update -p src/LocadoraFilmes.Api
```

Observação sobre SQLite: valores `decimal` e `DateTimeOffset` são armazenados como texto; por isso as consultas
evitam `ORDER BY`/`SUM` sobre esses tipos (ordenação por `Id`/`Titulo` e somas feitas em memória).
