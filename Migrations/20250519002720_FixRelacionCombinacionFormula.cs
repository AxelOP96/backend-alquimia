using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacionCombinacionFormula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Combinaciones_CombinacionId1",
                table: "Formulas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_CombinacionId1",
                table: "Formulas");

            migrationBuilder.DropColumn(
                name: "CombinacionId1",
                table: "Formulas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CombinacionId1",
                table: "Formulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_CombinacionId1",
                table: "Formulas",
                column: "CombinacionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Combinaciones_CombinacionId1",
                table: "Formulas",
                column: "CombinacionId1",
                principalTable: "Combinaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
