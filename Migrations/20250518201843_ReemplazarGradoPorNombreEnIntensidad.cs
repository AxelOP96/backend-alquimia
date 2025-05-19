using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class ReemplazarGradoPorNombreEnIntensidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grado",
                table: "Intensidades");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Intensidades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Intensidades");

            migrationBuilder.AddColumn<int>(
                name: "Grado",
                table: "Intensidades",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
