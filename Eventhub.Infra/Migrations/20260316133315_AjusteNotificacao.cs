using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AjusteNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                table: "Notificacao");

            migrationBuilder.AlterColumn<string>(
                name: "Prioridade",
                table: "Notificacao",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NotificacaoTipoId1",
                table: "Notificacao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_NotificacaoTipoId1",
                table: "Notificacao",
                column: "NotificacaoTipoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                table: "Notificacao",
                column: "NotificacaoTipoId",
                principalTable: "NotificacaoTipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId1",
                table: "Notificacao",
                column: "NotificacaoTipoId1",
                principalTable: "NotificacaoTipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                table: "Notificacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId1",
                table: "Notificacao");

            migrationBuilder.DropIndex(
                name: "IX_Notificacao_NotificacaoTipoId1",
                table: "Notificacao");

            migrationBuilder.DropColumn(
                name: "NotificacaoTipoId1",
                table: "Notificacao");

            migrationBuilder.AlterColumn<int>(
                name: "Prioridade",
                table: "Notificacao",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                table: "Notificacao",
                column: "NotificacaoTipoId",
                principalTable: "NotificacaoTipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
