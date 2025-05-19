using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    public partial class EliminarColumnasDuplicadasFormula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar Foreign Keys
            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Intensidades_IntensidadId1",
                table: "Formulas");

            // Eliminar índices
            migrationBuilder.DropIndex(
                name: "IX_Formulas_IntensidadId1",
                table: "Formulas");

            // Eliminar las columnas duplicadas
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
