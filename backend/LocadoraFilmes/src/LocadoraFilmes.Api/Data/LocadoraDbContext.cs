using LocadoraFilmes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Data;

public class LocadoraDbContext : DbContext
{
    public LocadoraDbContext(DbContextOptions<LocadoraDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<Filme> Filmes => Set<Filme>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Locacao> Locacoes => Set<Locacao>();
    public DbSet<LocacaoItem> LocacaoItens => Set<LocacaoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocadoraDbContext).Assembly);
    }
}
