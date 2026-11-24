using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Domain.Enums;
using LocadoraFilmes.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Data;

/// <summary>
/// Carga inicial de dados (idempotente: só insere quando a respectiva tabela está vazia).
/// Credenciais de desenvolvimento: admin@locadora.com / Admin@123 e atendente@locadora.com / Atendente@123.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(LocadoraDbContext db, ISenhaHasher hasher, CancellationToken ct = default)
    {
        await SeedUsuariosAsync(db, hasher, ct);
        await SeedCatalogoAsync(db, ct);
        await SeedClientesELocacoesAsync(db, ct);
    }

    private static async Task SeedUsuariosAsync(LocadoraDbContext db, ISenhaHasher hasher, CancellationToken ct)
    {
        if (await db.Usuarios.AnyAsync(ct))
        {
            return;
        }

        var agora = DateTimeOffset.UtcNow;

        db.Usuarios.AddRange(
            new Usuario
            {
                Nome = "Administrador",
                Email = "admin@locadora.com",
                SenhaHash = hasher.Gerar("Admin@123"),
                Perfil = Perfis.Admin,
                CriadoEm = agora
            },
            new Usuario
            {
                Nome = "Atendente Padrão",
                Email = "atendente@locadora.com",
                SenhaHash = hasher.Gerar("Atendente@123"),
                Perfil = Perfis.Atendente,
                CriadoEm = agora
            });

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedCatalogoAsync(LocadoraDbContext db, CancellationToken ct)
    {
        if (await db.Generos.AnyAsync(ct))
        {
            return;
        }

        var acao = new Genero { Nome = "Ação" };
        var comedia = new Genero { Nome = "Comédia" };
        var drama = new Genero { Nome = "Drama" };
        var ficcao = new Genero { Nome = "Ficção Científica" };
        var terror = new Genero { Nome = "Terror" };
        var animacao = new Genero { Nome = "Animação" };

        db.Generos.AddRange(acao, comedia, drama, ficcao, terror, animacao);

        var agora = DateTimeOffset.UtcNow;

        Filme Novo(string titulo, string sinopse, int ano, int duracao, string classificacao,
            decimal preco, int quantidade, Genero genero) => new()
        {
            Titulo = titulo,
            Sinopse = sinopse,
            AnoLancamento = ano,
            DuracaoMinutos = duracao,
            ClassificacaoIndicativa = classificacao,
            PrecoDiaria = preco,
            QuantidadeTotal = quantidade,
            QuantidadeDisponivel = quantidade,
            CriadoEm = agora,
            Genero = genero
        };

        db.Filmes.AddRange(
            Novo("Matrix", "Um programador descobre que a realidade é uma simulação.", 1999, 136, "14", 6.50m, 5, ficcao),
            Novo("Interestelar", "Exploradores viajam por um buraco de minhoca em busca de um novo lar.", 2014, 169, "10", 7.00m, 4, ficcao),
            Novo("O Poderoso Chefão", "A saga de uma família mafiosa italiana nos Estados Unidos.", 1972, 175, "16", 5.00m, 3, drama),
            Novo("Cidade de Deus", "A vida e o crime em uma favela do Rio de Janeiro.", 2002, 130, "18", 5.50m, 3, drama),
            Novo("Toy Story", "Brinquedos ganham vida quando os humanos não estão olhando.", 1995, 81, "L", 4.50m, 6, animacao),
            Novo("Shrek", "Um ogro embarca em uma jornada para resgatar uma princesa.", 2001, 90, "L", 4.50m, 4, animacao),
            Novo("Duro de Matar", "Um policial enfrenta terroristas em um arranha-céu.", 1988, 132, "16", 5.00m, 3, acao),
            Novo("Mad Max: Estrada da Fúria", "Uma perseguição implacável pelo deserto pós-apocalíptico.", 2015, 120, "16", 7.50m, 3, acao),
            Novo("O Auto da Compadecida", "As aventuras de João Grilo e Chicó no sertão nordestino.", 2000, 104, "12", 4.50m, 4, comedia),
            Novo("O Iluminado", "Um escritor enlouquece em um hotel isolado no inverno.", 1980, 146, "16", 5.50m, 2, terror));

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedClientesELocacoesAsync(LocadoraDbContext db, CancellationToken ct)
    {
        if (await db.Clientes.AnyAsync(ct))
        {
            return;
        }

        var agora = DateTimeOffset.UtcNow;

        var ana = new Cliente
        {
            Nome = "Ana Souza",
            Cpf = "52998224725",
            Email = "ana.souza@email.com",
            Telefone = "11987654321",
            DataNascimento = new DateOnly(1990, 5, 12),
            CriadoEm = agora
        };

        var bruno = new Cliente
        {
            Nome = "Bruno Lima",
            Cpf = "11144477735",
            Email = "bruno.lima@email.com",
            Telefone = "21991234567",
            DataNascimento = new DateOnly(1985, 11, 30),
            CriadoEm = agora
        };

        var carla = new Cliente
        {
            Nome = "Carla Mendes",
            Cpf = "16899535009",
            Email = "carla.mendes@email.com",
            Telefone = "31988887777",
            DataNascimento = new DateOnly(2000, 2, 20),
            CriadoEm = agora
        };

        db.Clientes.AddRange(ana, bruno, carla);

        if (!await db.Locacoes.AnyAsync(ct))
        {
            var filmes = await db.Filmes
                .Where(f => f.Titulo == "Matrix" || f.Titulo == "Toy Story" || f.Titulo == "Interestelar")
                .ToDictionaryAsync(f => f.Titulo, ct);

            if (filmes.Count == 3)
            {
                // Locação já devolvida no prazo (sem multa).
                var devolvida = new Locacao
                {
                    Cliente = ana,
                    DataLocacao = agora.AddDays(-10),
                    DataPrevistaDevolucao = agora.AddDays(-7),
                    DataDevolucao = agora.AddDays(-7),
                    Status = StatusLocacao.Devolvida,
                    ValorTotal = (6.50m + 4.50m) * 3,
                    ValorMulta = 0m
                };
                devolvida.Itens.Add(new LocacaoItem { Filme = filmes["Matrix"], PrecoDiaria = 6.50m });
                devolvida.Itens.Add(new LocacaoItem { Filme = filmes["Toy Story"], PrecoDiaria = 4.50m });

                // Locação ativa (baixa o estoque de Interestelar).
                var ativa = new Locacao
                {
                    Cliente = bruno,
                    DataLocacao = agora.AddDays(-1),
                    DataPrevistaDevolucao = agora.AddDays(4),
                    Status = StatusLocacao.Ativa,
                    ValorTotal = 7.00m * 5,
                    ValorMulta = 0m
                };
                ativa.Itens.Add(new LocacaoItem { Filme = filmes["Interestelar"], PrecoDiaria = 7.00m });
                filmes["Interestelar"].QuantidadeDisponivel -= 1;

                db.Locacoes.AddRange(devolvida, ativa);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
