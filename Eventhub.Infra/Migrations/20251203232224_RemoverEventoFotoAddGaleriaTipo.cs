using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoverEventoFotoAddGaleriaTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoFoto");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Galeria",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Galeria")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Evento",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Galeria");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Evento");

            migrationBuilder.CreateTable(
                name: "EventoFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoFoto_Evento_Id",
                        column: x => x.Id,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoFoto_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EventoFoto_IdFoto",
                table: "EventoFoto",
                column: "IdFoto");
        }
    }
}
