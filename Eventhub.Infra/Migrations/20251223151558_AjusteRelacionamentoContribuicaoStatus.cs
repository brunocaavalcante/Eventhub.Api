using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRelacionamentoContribuicaoStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContribuicaoPresente");

            migrationBuilder.AlterColumn<int>(
                name: "StatusContribuicaoId",
                table: "ContribuicaoPresente",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdStatusContribuicao",
                table: "ContribuicaoPresente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContribuicaoPresente_IdStatusContribuicao",
                table: "ContribuicaoPresente",
                column: "IdStatusContribuicao");

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_IdStatusContribuicao",
                table: "ContribuicaoPresente",
                column: "IdStatusContribuicao",
                principalTable: "StatusContribuicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente",
                column: "StatusContribuicaoId",
                principalTable: "StatusContribuicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_IdStatusContribuicao",
                table: "ContribuicaoPresente");

            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente");

            migrationBuilder.DropIndex(
                name: "IX_ContribuicaoPresente_IdStatusContribuicao",
                table: "ContribuicaoPresente");

            migrationBuilder.DropColumn(
                name: "IdStatusContribuicao",
                table: "ContribuicaoPresente");

            migrationBuilder.AlterColumn<int>(
                name: "StatusContribuicaoId",
                table: "ContribuicaoPresente",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContribuicaoPresente",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                table: "ContribuicaoPresente",
                column: "StatusContribuicaoId",
                principalTable: "StatusContribuicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
