using LocadoraFilmes.Api.Domain.Entities;

namespace LocadoraFilmes.Api.Dtos;

public static class Mapeamentos
{
    public static UsuarioResponse ToResponse(this Usuario u) => new(u.Id, u.Nome, u.Email, u.Perfil);

    public static GeneroResponse ToResponse(this Genero g) => new(g.Id, g.Nome);

    public static FilmeResponse ToResponse(this Filme f) => new(
        f.Id, f.Titulo, f.Sinopse, f.Diretor, f.AnoLancamento, f.DuracaoMinutos, f.ClassificacaoIndicativa,
        f.PrecoDiaria, f.QuantidadeTotal, f.QuantidadeDisponivel, f.GeneroId, f.Genero.Nome);

    public static ClienteResponse ToResponse(this Cliente c) => new(
        c.Id, c.Nome, c.Cpf, c.Email, c.Telefone, c.DataNascimento);

    public static LocacaoResponse ToResponse(this Locacao l) => new(
        l.Id,
        l.ClienteId,
        l.Cliente.Nome,
        l.DataLocacao,
        l.DataPrevistaDevolucao,
        l.DataDevolucao,
        l.ValorTotal,
        l.ValorMulta,
        l.Status,
        l.Itens.Select(i => new LocacaoItemResponse(i.FilmeId, i.Filme.Titulo, i.PrecoDiaria)).ToList());
}
