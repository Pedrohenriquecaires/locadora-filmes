using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Dtos;
using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Api.Services.Interfaces;
using LocadoraFilmes.Validation;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Services;

public class ClienteService : IClienteService
{
    private readonly LocadoraDbContext _db;
    private readonly IValidator<IClienteDados> _validator;

    public ClienteService(LocadoraDbContext db, IValidator<IClienteDados> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(string? nome, CancellationToken ct = default)
    {
        IQueryable<Cliente> consulta = _db.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim();
            consulta = consulta.Where(c => c.Nome.Contains(termo));
        }

        var clientes = await consulta.OrderBy(c => c.Nome).ToListAsync(ct);
        return clientes.Select(c => c.ToResponse()).ToList();
    }

    public async Task<ClienteResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var cliente = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Cliente {id} não encontrado.");

        return cliente.ToResponse();
    }

    public async Task<ClienteResponse> CriarAsync(ClienteRequest request, CancellationToken ct = default)
    {
        _validator.Validar(request).LancarSeInvalido();

        var cpf = ValidadorCpf.Normalizar(request.Cpf);
        var email = request.Email!.Trim().ToLowerInvariant();

        await GarantirUnicidadeAsync(null, cpf, email, ct);

        var cliente = new Cliente { CriadoEm = DateTimeOffset.UtcNow };
        Preencher(cliente, request, cpf, email);

        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync(ct);

        return cliente.ToResponse();
    }

    public async Task<ClienteResponse> AtualizarAsync(int id, ClienteRequest request, CancellationToken ct = default)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Cliente {id} não encontrado.");

        _validator.Validar(request).LancarSeInvalido();

        var cpf = ValidadorCpf.Normalizar(request.Cpf);
        var email = request.Email!.Trim().ToLowerInvariant();

        await GarantirUnicidadeAsync(id, cpf, email, ct);

        Preencher(cliente, request, cpf, email);
        await _db.SaveChangesAsync(ct);

        return cliente.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new RecursoNaoEncontradoException($"Cliente {id} não encontrado.");

        if (await _db.Locacoes.AnyAsync(l => l.ClienteId == id, ct))
        {
            throw new ConflitoException("Não é possível remover um cliente que possui locações registradas.");
        }

        _db.Clientes.Remove(cliente);
        await _db.SaveChangesAsync(ct);
    }

    private async Task GarantirUnicidadeAsync(int? idAtual, string cpf, string email, CancellationToken ct)
    {
        if (await _db.Clientes.AnyAsync(c => c.Id != idAtual && c.Cpf == cpf, ct))
        {
            throw new ConflitoException("Já existe um cliente cadastrado com este CPF.");
        }

        if (await _db.Clientes.AnyAsync(c => c.Id != idAtual && c.Email == email, ct))
        {
            throw new ConflitoException("Já existe um cliente cadastrado com este e-mail.");
        }
    }

    private static void Preencher(Cliente cliente, ClienteRequest request, string cpf, string email)
    {
        var telefone = ValidadorTelefone.Normalizar(request.Telefone);

        cliente.Nome = request.Nome!.Trim();
        cliente.Cpf = cpf;
        cliente.Email = email;
        cliente.Telefone = telefone.Length == 0 ? null : telefone;
        cliente.DataNascimento = request.DataNascimento!.Value;
    }
}
