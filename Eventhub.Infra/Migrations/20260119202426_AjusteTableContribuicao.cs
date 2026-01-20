using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AjusteTableContribuicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "ContribuicaoPresente");

            migrationBuilder.DropColumn(
                name: "LinkProduto",
                table: "ContribuicaoPresente");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "ContribuicaoPresente");

            migrationBuilder.RenameColumn(
                name: "StatusContribuicaoId",
                table: "ContribuicaoPresente",
                newName: "IdFoto");

            migrationBuilder.RenameIndex(
                name: "IX_ContribuicaoPresente_StatusContribuicaoId",
                table: "ContribuicaoPresente",
                newName: "IX_ContribuicaoPresente_IdFoto");

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_Fotos_IdFoto",
                table: "ContribuicaoPresente",
                column: "IdFoto",
                principalTable: "Fotos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_Fotos_IdFoto",
                table: "ContribuicaoPresente");

            migrationBuilder.RenameColumn(
                name: "IdFoto",
                table: "ContribuicaoPresente",
                newName: "StatusContribuicaoId");

            migrationBuilder.RenameIndex(
                name: "IX_ContribuicaoPresente_IdFoto",
                table: "ContribuicaoPresente",
                newName: "IX_ContribuicaoPresente_StatusContribuicaoId");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "ContribuicaoPresente",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LinkProduto",
                table: "ContribuicaoPresente",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "ContribuicaoPresente",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente",
                column: "StatusContribuicaoId",
                principalTable: "StatusContribuicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
