using System.Text;
using System.Text.Json.Serialization;
using LocadoraFilmes.Api.Data;
using LocadoraFilmes.Api.Handlers;
using LocadoraFilmes.Api.Security;
using LocadoraFilmes.Api.Services;
using LocadoraFilmes.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Configurações
// ---------------------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection(JwtSettings.Secao);
var jwt = jwtSection.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração 'Jwt' não encontrada.");

if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
{
    throw new InvalidOperationException("A chave 'Jwt:Key' deve ter pelo menos 32 caracteres.");
}

builder.Services.Configure<JwtSettings>(jwtSection);

// ---------------------------------------------------------------------------
// Banco de dados (Entity Framework Core + SQLite)
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Locadora") ?? "Data Source=App_Data/locadora.db";
var sqlite = new SqliteConnectionStringBuilder(connectionString);

if (!Path.IsPathRooted(sqlite.DataSource))
{
    sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);
}

Directory.CreateDirectory(Path.GetDirectoryName(sqlite.DataSource)!);

builder.Services.AddDbContext<LocadoraDbContext>(options => options.UseSqlite(sqlite.ToString()));

// ---------------------------------------------------------------------------
// Injeção de dependência: validação, segurança e serviços
// ---------------------------------------------------------------------------
builder.Services.AddLocadoraValidation();

builder.Services.AddSingleton<ISenhaHasher, SenhaHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IFilmeService, FilmeService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// ---------------------------------------------------------------------------
// Autenticação JWT e autorização
// ---------------------------------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Politicas.AdminOuAtendente, policy => policy.RequireRole(Perfis.Admin, Perfis.Atendente));
    options.AddPolicy(Politicas.ApenasAdmin, policy => policy.RequireRole(Perfis.Admin));

    // Qualquer endpoint sem [AllowAnonymous] exige usuário autenticado.
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

// ---------------------------------------------------------------------------
// CORS
// ---------------------------------------------------------------------------
const string politicaCors = "PoliticaCors";
var origensPermitidas = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(politicaCors, policy => policy
        .WithOrigins(origensPermitidas)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ---------------------------------------------------------------------------
// API (Controllers, JSON, tratamento global de erros)
// ---------------------------------------------------------------------------
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

// ---------------------------------------------------------------------------
// Migrations + Seed na inicialização
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LocadoraDbContext>();
    await db.Database.MigrateAsync();

    if (app.Configuration.GetValue("Seed:Habilitado", true))
    {
        var hasher = scope.ServiceProvider.GetRequiredService<ISenhaHasher>();
        await DbSeeder.SeedAsync(db, hasher);
    }
}

// ---------------------------------------------------------------------------
// Pipeline HTTP
// ---------------------------------------------------------------------------
app.UseExceptionHandler();
app.UseCors(politicaCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { nome = "Locadora de Filmes API", versao = "1.0.0", status = "online" }))
    .AllowAnonymous();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .AllowAnonymous();

app.MapControllers();

app.Run();
