using LocadoraFilmes.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraFilmes.Api.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nome).IsRequired().HasMaxLength(120);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(254);
        builder.Property(u => u.SenhaHash).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Perfil).IsRequired().HasMaxLength(20);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class GeneroConfiguration : IEntityTypeConfiguration<Genero>
{
    public void Configure(EntityTypeBuilder<Genero> builder)
    {
        builder.ToTable("Generos");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Nome).IsRequired().HasMaxLength(60);
        builder.HasIndex(g => g.Nome).IsUnique();
    }
}

public class FilmeConfiguration : IEntityTypeConfiguration<Filme>
{
    public void Configure(EntityTypeBuilder<Filme> builder)
    {
        builder.ToTable("Filmes");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Titulo).IsRequired().HasMaxLength(150);
        builder.Property(f => f.Sinopse).HasMaxLength(1000);
        builder.Property(f => f.ClassificacaoIndicativa).IsRequired().HasMaxLength(3);

        builder.HasIndex(f => new { f.Titulo, f.AnoLancamento }).IsUnique();

        builder.HasOne(f => f.Genero)
            .WithMany(g => g.Filmes)
            .HasForeignKey(f => f.GeneroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Cpf).IsRequired().HasMaxLength(11);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(254);
        builder.Property(c => c.Telefone).HasMaxLength(11);

        builder.HasIndex(c => c.Cpf).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();
    }
}

public class LocacaoConfiguration : IEntityTypeConfiguration<Locacao>
{
    public void Configure(EntityTypeBuilder<Locacao> builder)
    {
        builder.ToTable("Locacoes");
        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.Cliente)
            .WithMany(c => c.Locacoes)
            .HasForeignKey(l => l.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LocacaoItemConfiguration : IEntityTypeConfiguration<LocacaoItem>
{
    public void Configure(EntityTypeBuilder<LocacaoItem> builder)
    {
        builder.ToTable("LocacaoItens");
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.Locacao)
            .WithMany(l => l.Itens)
            .HasForeignKey(i => i.LocacaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Filme)
            .WithMany(f => f.Itens)
            .HasForeignKey(i => i.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
