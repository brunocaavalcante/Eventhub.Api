using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class NovoFluxoConviteViaLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvioConvite");

            migrationBuilder.DropTable(
                name: "Convite");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataResposta",
                table: "Participante",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdStatusConvite",
                table: "Participante",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MensagemOrganizador",
                table: "Participante",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MotivoRecusa",
                table: "Participante",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "QtdAcompanhantes",
                table: "Participante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TokenConvite",
                table: "Evento",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Participante_IdStatusConvite",
                table: "Participante",
                column: "IdStatusConvite");

            migrationBuilder.AddForeignKey(
                name: "FK_Participante_StatusEnvioConvite_IdStatusConvite",
                table: "Participante",
                column: "IdStatusConvite",
                principalTable: "StatusEnvioConvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participante_StatusEnvioConvite_IdStatusConvite",
                table: "Participante");

            migrationBuilder.DropIndex(
                name: "IX_Participante_IdStatusConvite",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "DataResposta",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "IdStatusConvite",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "MensagemOrganizador",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "MotivoRecusa",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "QtdAcompanhantes",
                table: "Participante");

            migrationBuilder.DropColumn(
                name: "TokenConvite",
                table: "Evento");

            migrationBuilder.CreateTable(
                name: "Convite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Mensagem = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opacity = table.Column<int>(type: "int", nullable: false),
                    TemaConvite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convite_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Convite_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EnvioConvite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdConvite = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdParticipante = table.Column<int>(type: "int", nullable: false),
                    IdStatusEnvioConvite = table.Column<int>(type: "int", nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MensagemResposta = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QtdAcompanhantes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioConvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvioConvite_Convite_IdConvite",
                        column: x => x.IdConvite,
                        principalTable: "Convite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvioConvite_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvioConvite_Participante_IdParticipante",
                        column: x => x.IdParticipante,
                        principalTable: "Participante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvioConvite_StatusEnvioConvite_IdStatusEnvioConvite",
                        column: x => x.IdStatusEnvioConvite,
                        principalTable: "StatusEnvioConvite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Convite_IdEvento",
                table: "Convite",
                column: "IdEvento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Convite_IdFoto",
                table: "Convite",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdConvite",
                table: "EnvioConvite",
                column: "IdConvite");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdEvento",
                table: "EnvioConvite",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdParticipante",
                table: "EnvioConvite",
                column: "IdParticipante");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdStatusEnvioConvite",
                table: "EnvioConvite",
                column: "IdStatusEnvioConvite");
        }
    }
}
