# Arquitetura — Locadora de Filmes

## Camadas

```mermaid
flowchart TB
    subgraph FRONT["Front-end · Angular standalone (frontend/)"]
        P["Páginas<br/>Login · Dashboard · Clientes · Filmes · Locações"]
        G["authGuard<br/>(guardas/)"]
        S["Services HttpClient<br/>(servicos/)"]
        I["authInterceptor<br/>anexa Bearer token"]
        P --> S --> I
        G -.protege.-> P
    end

    subgraph API["Back-end · ASP.NET Core Web API (LocadoraFilmes.Api)"]
        MW["Pipeline: ExceptionHandler · CORS · JWT · Authorization"]
        C["Controllers<br/>recebem a requisição e devolvem a resposta"]
        SV["Services<br/>regras de negócio"]
        DB["LocadoraDbContext<br/>EF Core · Migrations · Seed"]
        MW --> C --> SV --> DB
    end

    subgraph LIB["Class Library · LocadoraFilmes.Validation (pacote NuGet 1.1.0)"]
        V["Validadores e regras<br/>CPF · e-mail · telefone · senha · idade · diretor"]
    end

    T["LocadoraFilmes.Validation.Tests<br/>xUnit"]
    FEED[("Feed NuGet local<br/>nuget-local/ · 1.0.0 e 1.1.0")]
    SQL[("SQLite<br/>App_Data/locadora.db")]

    I -- "HTTP + JSON (JWT)" --> MW
    SV -- "valida DTOs" --> V
    DB --> SQL
    T -- "testa" --> V
    LIB -- "dotnet pack / push" --> FEED
    FEED -- "PackageReference" --> API
```

### Responsabilidades

| Camada | Responsabilidade | Não faz |
|--------|------------------|---------|
| **Controllers** | Recebem a requisição, chamam o Service e devolvem o status HTTP (200/201/204) | Regras de negócio ou acesso ao banco |
| **Services** | Regras de negócio (estoque, multa, limite de locações, relatório) e validação via biblioteca | Conhecer detalhes de HTTP |
| **Biblioteca de validação** | Regras de validação de cada entidade, reutilizáveis e testadas | Depender de ASP.NET Core ou EF Core |
| **DbContext / Migrations** | Mapeamento das entidades e evolução do esquema do banco | — |
| **ApiExceptionHandler** | Converte exceções em `ProblemDetails`: validação → 400, não encontrado → 404, conflito → 409, regra de negócio → 422 | — |

### Fluxo de uma requisição (ex.: cadastrar cliente)

```mermaid
sequenceDiagram
    actor U as Usuário
    participant A as Angular (ClienteForm)
    participant In as authInterceptor
    participant Ct as ClientesController
    participant Sv as ClienteService
    participant V as ClienteValidator (NuGet)
    participant Db as DbContext / SQLite

    U->>A: preenche e salva
    A->>In: POST /api/clientes
    In->>Ct: + Authorization: Bearer <token>
    Ct->>Sv: CriarAsync(request)
    Sv->>V: Validar(request)
    alt dados inválidos
        V-->>Sv: erros
        Sv-->>A: 400 com mensagens da biblioteca
        A-->>U: mostra os erros na tela
    else válido
        Sv->>Db: Add + SaveChanges
        Sv-->>Ct: ClienteResponse
        Ct-->>A: 201 Created + Location
    end
```

## Entidades

```mermaid
erDiagram
    GENERO ||--o{ FILME : "classifica"
    CLIENTE ||--o{ LOCACAO : "faz"
    LOCACAO ||--|{ LOCACAO_ITEM : "contém"
    FILME ||--o{ LOCACAO_ITEM : "é alugado em"

    GENERO {
        int Id PK
        string Nome
    }
    FILME {
        int Id PK
        string Titulo
        string Sinopse
        string Diretor "migration AdicionaDiretorFilme"
        int AnoLancamento
        int DuracaoMinutos
        string ClassificacaoIndicativa
        decimal PrecoDiaria
        int QuantidadeTotal
        int QuantidadeDisponivel
        int GeneroId FK
    }
    CLIENTE {
        int Id PK
        string Nome
        string Cpf "único"
        string Email "único"
        string Telefone
        date DataNascimento
    }
    LOCACAO {
        int Id PK
        int ClienteId FK
        datetime DataLocacao
        datetime DataPrevistaDevolucao
        datetime DataDevolucao
        decimal ValorTotal
        decimal ValorMulta
        string Status "Ativa | Devolvida"
    }
    LOCACAO_ITEM {
        int Id PK
        int LocacaoId FK
        int FilmeId FK
        decimal PrecoDiaria
    }
    USUARIO {
        int Id PK
        string Nome
        string Email "único"
        string SenhaHash
        string Perfil "Admin | Atendente"
    }
```

## Histórico de migrations

| Migration | Mudança |
|-----------|---------|
| `Inicial` | Criação das tabelas Usuarios, Generos, Filmes, Clientes, Locacoes e LocacaoItens |
| `AdicionaDiretorFilme` | Evolução do modelo: coluna `Diretor` na tabela Filmes |
