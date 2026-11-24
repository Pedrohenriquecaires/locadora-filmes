using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LocadoraFilmes.Api.Domain.Entities;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LocadoraFilmes.Api.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> opcoes)
    {
        _jwt = opcoes.Value;
    }

    public (string Token, DateTimeOffset ExpiraEm) Gerar(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var agora = DateTime.UtcNow;
        var expira = agora.AddMinutes(_jwt.ExpiracaoMinutos);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: agora,
            expires: expira,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), new DateTimeOffset(expira, TimeSpan.Zero));
    }
}
