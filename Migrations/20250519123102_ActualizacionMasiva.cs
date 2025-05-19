using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAlquimia.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionMasiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opinion_Usuarios_UsuarioId",
                table: "Opinion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Opinion",
                table: "Opinion");

            migrationBuilder.RenameTable(
                name: "Opinion",
                newName: "Opiniones");

            migrationBuilder.RenameIndex(
                name: "IX_Opinion_UsuarioId",
                table: "Opiniones",
                newName: "IX_Opiniones_UsuarioId");

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlDeRedireccionHaciaLaPaginaDondeSePuedeComprar",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "PiramideOlfativa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "Notas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "FamiliasOlfativas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opiniones",
                table: "Opiniones",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorEtiqueta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipografia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlturaLetra = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Opiniones_Usuarios_UsuarioId",
                table: "Opiniones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opiniones_Usuarios_UsuarioId",
                table: "Opiniones");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Opiniones",
                table: "Opiniones");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UrlDeRedireccionHaciaLaPaginaDondeSePuedeComprar",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "PiramideOlfativa");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "Notas");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "FamiliasOlfativas");

            migrationBuilder.RenameTable(
                name: "Opiniones",
                newName: "Opinion");

            migrationBuilder.RenameIndex(
                name: "IX_Opiniones_UsuarioId",
                table: "Opinion",
                newName: "IX_Opinion_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opinion",
                table: "Opinion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Opinion_Usuarios_UsuarioId",
                table: "Opinion",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
