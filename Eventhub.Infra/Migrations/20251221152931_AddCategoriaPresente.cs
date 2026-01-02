using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaPresente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCategoria",
                table: "Presente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoriaPresente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaPresente", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Presente_IdCategoria",
                table: "Presente",
                column: "IdCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Presente_CategoriaPresente_IdCategoria",
                table: "Presente",
                column: "IdCategoria",
                principalTable: "CategoriaPresente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presente_CategoriaPresente_IdCategoria",
                table: "Presente");

            migrationBuilder.DropTable(
                name: "CategoriaPresente");

            migrationBuilder.DropIndex(
                name: "IX_Presente_IdCategoria",
                table: "Presente");

            migrationBuilder.DropColumn(
                name: "IdCategoria",
                table: "Presente");
        }
    }
}
