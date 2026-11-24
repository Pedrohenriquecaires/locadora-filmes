using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraFilmes.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDiretorFilme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diretor",
                table: "Filmes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diretor",
                table: "Filmes");
        }
    }
}
