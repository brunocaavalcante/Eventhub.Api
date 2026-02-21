using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnReservaPresente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataReserva",
                table: "Presente",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdParticipanteReservou",
                table: "Presente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParticipanteReservouId",
                table: "Presente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Presente_ParticipanteReservouId",
                table: "Presente",
                column: "ParticipanteReservouId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presente_Participante_ParticipanteReservouId",
                table: "Presente",
                column: "ParticipanteReservouId",
                principalTable: "Participante",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presente_Participante_ParticipanteReservouId",
                table: "Presente");

            migrationBuilder.DropIndex(
                name: "IX_Presente_ParticipanteReservouId",
                table: "Presente");

            migrationBuilder.DropColumn(
                name: "DataReserva",
                table: "Presente");

            migrationBuilder.DropColumn(
                name: "IdParticipanteReservou",
                table: "Presente");

            migrationBuilder.DropColumn(
                name: "ParticipanteReservouId",
                table: "Presente");
        }
    }
}
