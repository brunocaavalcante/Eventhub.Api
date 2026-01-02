using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarGaleriaPresenteRelacionamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presente_Fotos_IdFoto",
                table: "Presente");

            migrationBuilder.DropIndex(
                name: "IX_Presente_IdFoto",
                table: "Presente");

            migrationBuilder.DropColumn(
                name: "IdFoto",
                table: "Presente");

            migrationBuilder.AddColumn<int>(
                name: "IdPresente",
                table: "Galeria",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Galeria_IdPresente",
                table: "Galeria",
                column: "IdPresente");

            migrationBuilder.AddForeignKey(
                name: "FK_Galeria_Presente_IdPresente",
                table: "Galeria",
                column: "IdPresente",
                principalTable: "Presente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galeria_Presente_IdPresente",
                table: "Galeria");

            migrationBuilder.DropIndex(
                name: "IX_Galeria_IdPresente",
                table: "Galeria");

            migrationBuilder.DropColumn(
                name: "IdPresente",
                table: "Galeria");

            migrationBuilder.AddColumn<int>(
                name: "IdFoto",
                table: "Presente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Presente_IdFoto",
                table: "Presente",
                column: "IdFoto");

            migrationBuilder.AddForeignKey(
                name: "FK_Presente_Fotos_IdFoto",
                table: "Presente",
                column: "IdFoto",
                principalTable: "Fotos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
