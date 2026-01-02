using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ViculoUsuarioFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Usuario");

            migrationBuilder.AddColumn<int>(
                name: "IdFoto",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdFoto",
                table: "Usuario",
                column: "IdFoto");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Fotos_IdFoto",
                table: "Usuario",
                column: "IdFoto",
                principalTable: "Fotos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Fotos_IdFoto",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_IdFoto",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "IdFoto",
                table: "Usuario");

            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Usuario",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
