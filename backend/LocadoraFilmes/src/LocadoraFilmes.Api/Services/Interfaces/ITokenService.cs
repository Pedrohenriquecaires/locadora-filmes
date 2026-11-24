using LocadoraFilmes.Api.Domain.Entities;

namespace LocadoraFilmes.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiraEm) Gerar(Usuario usuario);
}
