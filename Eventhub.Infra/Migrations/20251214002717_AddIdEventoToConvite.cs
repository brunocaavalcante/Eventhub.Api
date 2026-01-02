using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddIdEventoToConvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEvento",
                table: "Convite",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Convite_IdEvento",
                table: "Convite",
                column: "IdEvento",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Convite_Evento_IdEvento",
                table: "Convite",
                column: "IdEvento",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Convite_Evento_IdEvento",
                table: "Convite");

            migrationBuilder.DropIndex(
                name: "IX_Convite_IdEvento",
                table: "Convite");

            migrationBuilder.DropColumn(
                name: "IdEvento",
                table: "Convite");
        }
    }
}
