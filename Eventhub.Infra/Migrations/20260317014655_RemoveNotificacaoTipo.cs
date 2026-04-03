using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotificacaoTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                table: "Notificacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId1",
                table: "Notificacao");

            migrationBuilder.DropTable(
                name: "NotificacaoTipo");

            migrationBuilder.DropIndex(
                name: "IX_Notificacao_NotificacaoTipoId",
                table: "Notificacao");

            migrationBuilder.DropIndex(
                name: "IX_Notificacao_NotificacaoTipoId1",
                table: "Notificacao");

            migrationBuilder.DropColumn(
                name: "NotificacaoTipoId",
                table: "Notificacao");

            migrationBuilder.DropColumn(
                name: "NotificacaoTipoId1",
                table: "Notificacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificacaoTipoId",
                table: "Notificacao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificacaoTipoId1",
                table: "Notificacao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificacaoTipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FotosId = table.Column<int>(type: "int", nullable: true),
                    IconePadrao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoPadrao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacaoTipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacaoTipo_Fotos_FotosId",
                        column: x => x.FotosId,
                        principalTable: "Fotos",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_NotificacaoTipoId",
                table: "Notificacao",
                column: "NotificacaoTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_NotificacaoTipoId1",
                table: "Notificacao",
                column: "NotificacaoTipoId1");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoTipo_FotosId",
                table: "NotificacaoTipo",
                column: "FotosId");

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
    }
}
