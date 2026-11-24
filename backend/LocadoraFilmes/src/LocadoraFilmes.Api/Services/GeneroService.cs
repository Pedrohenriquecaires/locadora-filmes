using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Services.Interfaces;
using LocadoraFilmes.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

public class GeneroService : IGeneroService
{
    private readonly LocadoraDbContext _db;
    private readonly IValidator<IGeneroDados> _validator;

    public GeneroService(LocadoraDbContext db, IValidator<IGeneroDados> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<GeneroResponse>> ListarAsync(CancellationToken ct = default)
    {
        var generos = await _db.Generos.AsNoTracking().OrderBy(g => g.Nome).ToListAsync(ct);
        return generos.Select(g => g.ToResponse()).ToList();
    }

    public async Task<GeneroResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var genero = await _db.Generos.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Gênero {id} não encontrado.");

        return genero.ToResponse();
    }

    public async Task<GeneroResponse> CriarAsync(GeneroRequest request, CancellationToken ct = default)
    {
        _validator.Validar(request).LancarSeInvalido();

        var nome = request.Nome!.Trim();

        if (await _db.Generos.AnyAsync(g => g.Nome == nome, ct))
        {
            throw new ConflitoException($"Já existe um gênero com o nome '{nome}'.");
        }

        var genero = new Genero { Nome = nome };
        _db.Generos.Add(genero);
        await _db.SaveChangesAsync(ct);

        return genero.ToResponse();
    }

    public async Task<GeneroResponse> AtualizarAsync(int id, GeneroRequest request, CancellationToken ct = default)
    {
        var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Gênero {id} não encontrado.");

        _validator.Validar(request).LancarSeInvalido();

        var nome = request.Nome!.Trim();

        if (await _db.Generos.AnyAsync(g => g.Id != id && g.Nome == nome, ct))
        {
            throw new ConflitoException($"Já existe um gênero com o nome '{nome}'.");
        }

        genero.Nome = nome;
        await _db.SaveChangesAsync(ct);

        return genero.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Gênero {id} não encontrado.");

        if (await _db.Filmes.AnyAsync(f => f.GeneroId == id, ct))
        {
            throw new ConflitoException("Não é possível remover um gênero que possui filmes cadastrados.");
        }

        _db.Generos.Remove(genero);
        await _db.SaveChangesAsync(ct);
    }
}
