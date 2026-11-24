using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraFilmes.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    DataNascimento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Perfil = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Sinopse = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AnoLancamento = table.Column<int>(type: "INTEGER", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassificacaoIndicativa = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    PrecoDiaria = table.Column<decimal>(type: "TEXT", nullable: false),
                    QuantidadeTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    GeneroId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filmes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filmes_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataLocacao = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DataPrevistaDevolucao = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DataDevolucao = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ValorTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorMulta = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocacaoItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocacaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoDiaria = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocacaoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocacaoItens_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocacaoItens_Locacoes_LocacaoId",
                        column: x => x.LocacaoId,
                        principalTable: "Locacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cpf",
                table: "Clientes",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filmes_GeneroId",
                table: "Filmes",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Filmes_Titulo_AnoLancamento",
                table: "Filmes",
                columns: new[] { "Titulo", "AnoLancamento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Nome",
                table: "Generos",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocacaoItens_FilmeId",
                table: "LocacaoItens",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocacaoItens_LocacaoId",
                table: "LocacaoItens",
                column: "LocacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_ClienteId",
                table: "Locacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LocacaoItens");
            migrationBuilder.DropTable(name: "Locacoes");
            migrationBuilder.DropTable(name: "Filmes");
            migrationBuilder.DropTable(name: "Usuarios");
            migrationBuilder.DropTable(name: "Generos");
            migrationBuilder.DropTable(name: "Clientes");
        }
    }
}
