using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPixEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PixEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Finalidade = table.Column<int>(type: "int", nullable: false),
                    NomeBeneficiario = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QRCodePix = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PixEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PixEvento_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PixEvento_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PixEvento_EventoId",
                table: "PixEvento",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_PixEvento_IdEvento",
                table: "PixEvento",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_PixEvento_IdEvento_Finalidade",
                table: "PixEvento",
                columns: new[] { "IdEvento", "Finalidade" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PixEvento");
        }
    }
}
