using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusEnvioConvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EnvioConvite");

            migrationBuilder.AddColumn<int>(
                name: "IdStatusEnvioConvite",
                table: "EnvioConvite",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StatusEnvioConvite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEnvioConvite", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdStatusEnvioConvite",
                table: "EnvioConvite",
                column: "IdStatusEnvioConvite");

            migrationBuilder.AddForeignKey(
                name: "FK_EnvioConvite_StatusEnvioConvite_IdStatusEnvioConvite",
                table: "EnvioConvite",
                column: "IdStatusEnvioConvite",
                principalTable: "StatusEnvioConvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnvioConvite_StatusEnvioConvite_IdStatusEnvioConvite",
                table: "EnvioConvite");

            migrationBuilder.DropTable(
                name: "StatusEnvioConvite");

            migrationBuilder.DropIndex(
                name: "IX_EnvioConvite_IdStatusEnvioConvite",
                table: "EnvioConvite");

            migrationBuilder.DropColumn(
                name: "IdStatusEnvioConvite",
                table: "EnvioConvite");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EnvioConvite",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
