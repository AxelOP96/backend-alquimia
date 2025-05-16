using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionProductoProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_FamiliasOlfativas_Creadores_CreadorId",
                table: "FamiliasOlfativas");

            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Creadores_CreadorId",
                table: "Formulas");

            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Creadores_CreadorId",
                table: "Notas");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Creadores_CreadorId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "Creadores");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "CreadorId",
                table: "Productos",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Productos_CreadorId",
                table: "Productos",
                newName: "IX_Productos_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "CreadorId",
                table: "Notas",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Notas_CreadorId",
                table: "Notas",
                newName: "IX_Notas_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "CreadorId",
                table: "FamiliasOlfativas",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_FamiliasOlfativas_CreadorId",
                table: "FamiliasOlfativas",
                newName: "IX_FamiliasOlfativas_UsuarioId");

            migrationBuilder.AddColumn<bool>(
                name: "EsProveedor",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdProveedor",
                table: "Productos",
                column: "IdProveedor");

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Usuarios_CreadorId",
                table: "CreacionesFinales",
                column: "CreadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliasOlfativas_Usuarios_UsuarioId",
                table: "FamiliasOlfativas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Usuarios_CreadorId",
                table: "Formulas",
                column: "CreadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Usuarios_UsuarioId",
                table: "Notas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Usuarios_IdProveedor",
                table: "Productos",
                column: "IdProveedor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Usuarios_UsuarioId",
                table: "Productos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreacionesFinales_Usuarios_CreadorId",
                table: "CreacionesFinales");

            migrationBuilder.DropForeignKey(
                name: "FK_FamiliasOlfativas_Usuarios_UsuarioId",
                table: "FamiliasOlfativas");

            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Usuarios_CreadorId",
                table: "Formulas");

            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Usuarios_UsuarioId",
                table: "Notas");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Usuarios_IdProveedor",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Usuarios_UsuarioId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_IdProveedor",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EsProveedor",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Productos",
                newName: "CreadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Productos_UsuarioId",
                table: "Productos",
                newName: "IX_Productos_CreadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Notas",
                newName: "CreadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Notas_UsuarioId",
                table: "Notas",
                newName: "IX_Notas_CreadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "FamiliasOlfativas",
                newName: "CreadorId");

            migrationBuilder.RenameIndex(
                name: "IX_FamiliasOlfativas_UsuarioId",
                table: "FamiliasOlfativas",
                newName: "IX_FamiliasOlfativas_CreadorId");

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Creadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creadores_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proveedores_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreacionesFinales_Creadores_CreadorId",
                table: "CreacionesFinales",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliasOlfativas_Creadores_CreadorId",
                table: "FamiliasOlfativas",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Creadores_CreadorId",
                table: "Formulas",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Creadores_CreadorId",
                table: "Notas",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Creadores_CreadorId",
                table: "Productos",
                column: "CreadorId",
                principalTable: "Creadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
