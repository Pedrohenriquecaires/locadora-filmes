using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Services.Interfaces;
using LocadoraFilmes.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

public class FilmeService : IFilmeService
{
    private readonly LocadoraDbContext _db;
    private readonly IValidator<IFilmeDados> _validator;

    public FilmeService(LocadoraDbContext db, IValidator<IFilmeDados> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<FilmeResponse>> ListarAsync(string? titulo, int? generoId, CancellationToken ct = default)
    {
        IQueryable<Filme> consulta = _db.Filmes.AsNoTracking().Include(f => f.Genero);

        if (!string.IsNullOrWhiteSpace(titulo))
        {
            var termo = titulo.Trim();
            consulta = consulta.Where(f => f.Titulo.Contains(termo));
        }

        if (generoId.HasValue)
        {
            consulta = consulta.Where(f => f.GeneroId == generoId.Value);
        }

        var filmes = await consulta.OrderBy(f => f.Titulo).ToListAsync(ct);
        return filmes.Select(f => f.ToResponse()).ToList();
    }

    public async Task<FilmeResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var filme = await _db.Filmes.AsNoTracking().Include(f => f.Genero).FirstOrDefaultAsync(f => f.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Filme {id} não encontrado.");

        return filme.ToResponse();
    }

    public async Task<FilmeResponse> CriarAsync(FilmeRequest request, CancellationToken ct = default)
    {
        _validator.Validar(request).LancarSeInvalido();
        ValidarDiretor(request.Diretor);

        var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == request.GeneroId, ct)
            ?? throw new ValidacaoException(nameof(request.GeneroId), "Gênero não encontrado.");

        var titulo = request.Titulo!.Trim();

        if (await _db.Filmes.AnyAsync(f => f.Titulo == titulo && f.AnoLancamento == request.AnoLancamento, ct))
        {
            throw new ConflitoException($"Já existe o filme '{titulo}' ({request.AnoLancamento}).");
        }

        var filme = new Filme
        {
            Titulo = titulo,
            Sinopse = string.IsNullOrWhiteSpace(request.Sinopse) ? null : request.Sinopse.Trim(),
            Diretor = string.IsNullOrWhiteSpace(request.Diretor) ? null : request.Diretor.Trim(),
            AnoLancamento = request.AnoLancamento,
            DuracaoMinutos = request.DuracaoMinutos,
            ClassificacaoIndicativa = request.ClassificacaoIndicativa!.Trim().ToUpperInvariant(),
            PrecoDiaria = request.PrecoDiaria,
            QuantidadeTotal = request.QuantidadeTotal,
            QuantidadeDisponivel = request.QuantidadeTotal,
            CriadoEm = DateTimeOffset.UtcNow,
            Genero = genero
        };

        _db.Filmes.Add(filme);
        await _db.SaveChangesAsync(ct);

        return filme.ToResponse();
    }

    public async Task<FilmeResponse> AtualizarAsync(int id, FilmeRequest request, CancellationToken ct = default)
    {
        var filme = await _db.Filmes.Include(f => f.Genero).FirstOrDefaultAsync(f => f.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Filme {id} não encontrado.");

        _validator.Validar(request).LancarSeInvalido();
        ValidarDiretor(request.Diretor);

        var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == request.GeneroId, ct)
            ?? throw new ValidacaoException(nameof(request.GeneroId), "Gênero não encontrado.");

        var titulo = request.Titulo!.Trim();

        if (await _db.Filmes.AnyAsync(f => f.Id != id && f.Titulo == titulo && f.AnoLancamento == request.AnoLancamento, ct))
        {
            throw new ConflitoException($"Já existe o filme '{titulo}' ({request.AnoLancamento}).");
        }

        var emUso = filme.QuantidadeTotal - filme.QuantidadeDisponivel;
        if (request.QuantidadeTotal < emUso)
        {
            throw new ConflitoException(
                $"A quantidade total não pode ser menor que o número de cópias atualmente locadas ({emUso}).");
        }

        filme.Titulo = titulo;
        filme.Sinopse = string.IsNullOrWhiteSpace(request.Sinopse) ? null : request.Sinopse.Trim();
        filme.Diretor = string.IsNullOrWhiteSpace(request.Diretor) ? null : request.Diretor.Trim();
        filme.AnoLancamento = request.AnoLancamento;
        filme.DuracaoMinutos = request.DuracaoMinutos;
        filme.ClassificacaoIndicativa = request.ClassificacaoIndicativa!.Trim().ToUpperInvariant();
        filme.PrecoDiaria = request.PrecoDiaria;
        filme.QuantidadeTotal = request.QuantidadeTotal;
        filme.QuantidadeDisponivel = request.QuantidadeTotal - emUso;
        filme.Genero = genero;

        await _db.SaveChangesAsync(ct);

        return filme.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var filme = await _db.Filmes.FirstOrDefaultAsync(f => f.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Filme {id} não encontrado.");

        if (await _db.LocacaoItens.AnyAsync(i => i.FilmeId == id, ct))
        {
            throw new ConflitoException("Não é possível remover um filme que possui locações registradas.");
        }

        _db.Filmes.Remove(filme);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Regra da biblioteca LocadoraFilmes.Validation 1.1.0 (ValidadorDiretor).</summary>
    private static void ValidarDiretor(string? diretor)
    {
        if (!ValidadorDiretor.EhValido(diretor))
        {
            throw new ValidacaoException(nameof(FilmeRequest.Diretor),
                $"O diretor deve ter de {ValidadorDiretor.TamanhoMinimo} a {ValidadorDiretor.TamanhoMaximo} caracteres " +
                "e conter apenas letras, espaços, ponto, apóstrofo ou hífen.");
        }
    }
}
