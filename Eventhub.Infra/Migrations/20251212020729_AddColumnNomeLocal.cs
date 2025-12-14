using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnNomeLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Ordem",
                table: "Galeria",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NomeLocal",
                table: "EnderecoEvento",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeLocal",
                table: "EnderecoEvento");

            migrationBuilder.AlterColumn<int>(
                name: "Ordem",
                table: "Galeria",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
