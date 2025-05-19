using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class QuitarIntensidadId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar el índice si existe
            migrationBuilder.DropIndex(
                name: "IX_Formulas_IntensidadId1",
                table: "Formulas");

            // Eliminar la columna sobrante
            migrationBuilder.DropColumn(
                name: "IntensidadId1",
                table: "Formulas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntensidadId1",
                table: "Formulas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_IntensidadId1",
                table: "Formulas",
                column: "IntensidadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Intensidades_IntensidadId1",
                table: "Formulas",
                column: "IntensidadId1",
                principalTable: "Intensidades",
                principalColumn: "Id");
        }

    }
}
