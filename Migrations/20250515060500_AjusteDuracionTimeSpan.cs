using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class AjusteDuracionTimeSpan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Formulas_IdFormula",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Pedidos_IdPedido",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Combinaciones_CombinacionId1",
                table: "Formulas");

            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Intensidades_IntensidadId1",
                table: "Formulas");

            migrationBuilder.DropForeignKey(
                name: "FK_Notas_PiramideOlfativa_SectorId",
                table: "Notas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_CombinacionId1",
                table: "Formulas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_IntensidadId1",
                table: "Formulas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PiramideOlfativa",
                table: "PiramideOlfativa");

            migrationBuilder.DropColumn(
                name: "CombinacionId1",
                table: "Formulas");

            migrationBuilder.DropColumn(
                name: "IntensidadId1",
                table: "Formulas");

            migrationBuilder.RenameTable(
                name: "PiramideOlfativa",
                newName: "Sectores");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Notas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Notas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sectores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Sectores",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sectores",
                table: "Sectores",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CompatibilidadFamiliasOlfativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FamiliaOlfativaAId = table.Column<int>(type: "int", nullable: false),
                    FamiliaOlfativaBId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompatibilidadFamiliasOlfativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompatibilidadFamiliasOlfativas_FamiliasOlfativas_FamiliaOlfativaAId",
                        column: x => x.FamiliaOlfativaAId,
                        principalTable: "FamiliasOlfativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibilidadFamiliasOlfativas_FamiliasOlfativas_FamiliaOlfativaBId",
                        column: x => x.FamiliaOlfativaBId,
                        principalTable: "FamiliasOlfativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "FamiliasOlfativas",
                columns: new[] { "Id", "CreadorId", "Description", "Nombre" },
                values: new object[,]
                {
                    { 1, null, "Notas frescas y chispeantes derivadas de cítricos.", "Cítrica" },
                    { 2, null, "Acordes dulces y jugosos de frutas no cítricas.", "Frutal" },
                    { 3, null, "Bouquet de flores de tallo, pétalos y capullos.", "Floral" },
                    { 4, null, "Acordes especiados, resinosos y cálidos.", "Oriental" },
                    { 5, null, "Esencias de maderas secas, resinosas o ahumadas.", "Amaderada" },
                    { 6, null, "Hierbas y plantas de aroma fresco y limpio.", "Aromática" },
                    { 7, null, "Hojas y tallos recién cortados, sensación herbal.", "Verde" },
                    { 8, null, "Notas comestibles, dulces y reconfortantes.", "Gourmand" },
                    { 9, null, "Acordes suaves, limpios y almizclados.", "Almizclada" },
                    { 10, null, "Sensación marina y de brisa fresca y húmeda.", "Acuática" }
                });

            migrationBuilder.InsertData(
                table: "Sectores",
                columns: new[] { "Id", "Description", "Duracion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Notas que se perciben inmediatamente.", new TimeSpan(0, 1, 0, 0, 0), "Salida" },
                    { 2, "Notas que definen el carácter del perfume.", new TimeSpan(0, 4, 0, 0, 0), "Corazón" },
                    { 3, "Notas de base, las más persistentes.", new TimeSpan(0, 8, 0, 0, 0), "Fondo" }
                });

            migrationBuilder.InsertData(
                table: "CompatibilidadFamiliasOlfativas",
                columns: new[] { "Id", "FamiliaOlfativaAId", "FamiliaOlfativaBId" },
                values: new object[,]
                {
                    { 1, 1, 3 },
                    { 2, 1, 6 },
                    { 3, 2, 3 },
                    { 4, 3, 5 },
                    { 5, 4, 5 },
                    { 6, 4, 9 },
                    { 7, 8, 4 },
                    { 8, 10, 1 }
                });

            migrationBuilder.InsertData(
                table: "Notas",
                columns: new[] { "Id", "CreadorId", "Descripcion", "FamiliaOlfativaId", "Nombre", "SectorId" },
                values: new object[,]
                {
                    { 1, null, "Cítrico verde, chispeante y ligeramente floral.", 1, "Bergamota", 1 },
                    { 2, null, "Cítrico luminoso y ácido, recuerda a la piel de limón.", 1, "Limón", 1 },
                    { 3, null, "Cítrico dulce y jugoso, con matiz infantil y alegre.", 1, "Mandarina", 1 },
                    { 4, null, "Frutal crujiente, faceta verde-dulce que aporta frescura.", 2, "Manzana", 1 },
                    { 5, null, "Fruta roja ácida y azucarada, da un tono juvenil.", 2, "Frambuesa", 1 },
                    { 6, null, "Floral clásico, aterciopelado y ligeramente especiado.", 3, "Rosa", 2 },
                    { 7, null, "Floral blanco, opulento, con matiz indólico y sensual.", 3, "Jazmín", 2 },
                    { 8, null, "Dulce, cremosa y envolvente; aporta calidez oriental.", 4, "Vainilla", 3 },
                    { 9, null, "Acorde resinoso-balsámico, aporta profundidad y dulzor.", 4, "Ámbar", 3 },
                    { 10, null, "Madera lactónica, suave y cremosa con faceta almizclada.", 5, "Sándalo", 3 },
                    { 11, null, "Madera seca, terrosa y ligeramente ahumada.", 5, "Cedro", 3 },
                    { 12, null, "Aromática herbal, calmante y limpia (toque alcanforado).", 6, "Lavanda", 2 },
                    { 13, null, "Aromática verde con punto terroso y fresco.", 6, "Salvia", 2 },
                    { 14, null, "Verde lechoso, natural y ligeramente afrutado.", 7, "Hoja de Higuera", 1 },
                    { 15, null, "Cacao profundo, cremoso y envolvente, efecto gourmand.", 8, "Chocolate", 3 },
                    { 16, null, "Suave, limpio y empolvado; fija y da sensación de piel.", 9, "Almizcle Blanco", 3 },
                    { 17, null, "Acorde ozónico-salado que evoca aire marino fresco.", 10, "Brisa Marina", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilidadFamiliasOlfativas_FamiliaOlfativaAId_FamiliaOlfativaBId",
                table: "CompatibilidadFamiliasOlfativas",
                columns: new[] { "FamiliaOlfativaAId", "FamiliaOlfativaBId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilidadFamiliasOlfativas_FamiliaOlfativaBId",
                table: "CompatibilidadFamiliasOlfativas",
                column: "FamiliaOlfativaBId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Formulas_IdFormula",
                table: "CreacionesFinales",
                column: "IdFormula",
                principalTable: "Formulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Pedidos_IdPedido",
                table: "CreacionesFinales",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Sectores_SectorId",
                table: "Notas",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Formulas_IdFormula",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Pedidos_IdPedido",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Sectores_SectorId",
                table: "Notas");

            migrationBuilder.DropTable(
                name: "CompatibilidadFamiliasOlfativas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sectores",
                table: "Sectores");

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Notas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FamiliasOlfativas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Sectores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sectores",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sectores",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Sectores");

            migrationBuilder.RenameTable(
                name: "Sectores",
                newName: "PiramideOlfativa");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Notas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Notas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<int>(
                name: "CombinacionId1",
                table: "Formulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntensidadId1",
                table: "Formulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PiramideOlfativa",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PiramideOlfativa",
                table: "PiramideOlfativa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_CombinacionId1",
                table: "Formulas",
                column: "CombinacionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_IntensidadId1",
                table: "Formulas",
                column: "IntensidadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Formulas_IdFormula",
                table: "CreacionesFinales",
                column: "IdFormula",
                principalTable: "Formulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Pedidos_IdPedido",
                table: "CreacionesFinales",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Combinaciones_CombinacionId1",
                table: "Formulas",
                column: "CombinacionId1",
                principalTable: "Combinaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Intensidades_IntensidadId1",
                table: "Formulas",
                column: "IntensidadId1",
                principalTable: "Intensidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_PiramideOlfativa_SectorId",
                table: "Notas",
                column: "SectorId",
                principalTable: "PiramideOlfativa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
