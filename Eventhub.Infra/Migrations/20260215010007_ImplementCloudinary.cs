using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ImplementCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base64",
                table: "Fotos");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Fotos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Fotos",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Fotos",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Fotos");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Fotos");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Fotos");

            migrationBuilder.AddColumn<string>(
                name: "Base64",
                table: "Fotos",
                type: "LONGTEXT",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
