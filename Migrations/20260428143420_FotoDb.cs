using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveNow.API.Migrations
{
    /// <inheritdoc />
    public partial class FotoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Clientes");
        }
    }
}
