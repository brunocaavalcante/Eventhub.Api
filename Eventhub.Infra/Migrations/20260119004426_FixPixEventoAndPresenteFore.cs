using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixPixEventoAndPresenteFore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PixEvento_Evento_EventoId",
                table: "PixEvento");

            migrationBuilder.DropIndex(
                name: "IX_PixEvento_EventoId",
                table: "PixEvento");

            migrationBuilder.DropColumn(
                name: "EventoId",
                table: "PixEvento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventoId",
                table: "PixEvento",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PixEvento_EventoId",
                table: "PixEvento",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PixEvento_Evento_EventoId",
                table: "PixEvento",
                column: "EventoId",
                principalTable: "Evento",
                principalColumn: "Id");
        }
    }
}
