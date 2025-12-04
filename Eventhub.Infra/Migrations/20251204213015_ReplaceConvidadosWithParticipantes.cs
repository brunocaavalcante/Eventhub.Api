using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceConvidadosWithParticipantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acompanhantes_Convidados_IdConvite",
                table: "Acompanhantes");

            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_Convidados_IdConvidado",
                table: "ContribuicaoPresente");

            migrationBuilder.DropForeignKey(
                name: "FK_EnvioConvite_Convidados_IdConvidado",
                table: "EnvioConvite");

            migrationBuilder.DropTable(
                name: "Convidados");

            migrationBuilder.RenameColumn(
                name: "IdConvidado",
                table: "EnvioConvite",
                newName: "IdParticipante");

            migrationBuilder.RenameIndex(
                name: "IX_EnvioConvite_IdConvidado",
                table: "EnvioConvite",
                newName: "IX_EnvioConvite_IdParticipante");

            migrationBuilder.RenameColumn(
                name: "IdConvidado",
                table: "ContribuicaoPresente",
                newName: "IdParticipante");

            migrationBuilder.RenameIndex(
                name: "IX_ContribuicaoPresente_IdConvidado",
                table: "ContribuicaoPresente",
                newName: "IX_ContribuicaoPresente_IdParticipante");

            migrationBuilder.RenameColumn(
                name: "IdConvite",
                table: "Acompanhantes",
                newName: "IdParticipante");

            migrationBuilder.RenameIndex(
                name: "IX_Acompanhantes_IdConvite",
                table: "Acompanhantes",
                newName: "IX_Acompanhantes_IdParticipante");

            migrationBuilder.CreateTable(
                name: "Participante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdPerfil = table.Column<int>(type: "int", nullable: false),
                    CadastroPendente = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participante_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participante_Perfil_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participante_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Participante_IdEvento_IdUsuario_IdPerfil",
                table: "Participante",
                columns: new[] { "IdEvento", "IdUsuario", "IdPerfil" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participante_IdPerfil",
                table: "Participante",
                column: "IdPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_Participante_IdUsuario",
                table: "Participante",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Acompanhantes_Participante_IdParticipante",
                table: "Acompanhantes",
                column: "IdParticipante",
                principalTable: "Participante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_Participante_IdParticipante",
                table: "ContribuicaoPresente",
                column: "IdParticipante",
                principalTable: "Participante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnvioConvite_Participante_IdParticipante",
                table: "EnvioConvite",
                column: "IdParticipante",
                principalTable: "Participante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acompanhantes_Participante_IdParticipante",
                table: "Acompanhantes");

            migrationBuilder.DropForeignKey(
                name: "FK_ContribuicaoPresente_Participante_IdParticipante",
                table: "ContribuicaoPresente");

            migrationBuilder.DropForeignKey(
                name: "FK_EnvioConvite_Participante_IdParticipante",
                table: "EnvioConvite");

            migrationBuilder.DropTable(
                name: "Participante");

            migrationBuilder.RenameColumn(
                name: "IdParticipante",
                table: "EnvioConvite",
                newName: "IdConvidado");

            migrationBuilder.RenameIndex(
                name: "IX_EnvioConvite_IdParticipante",
                table: "EnvioConvite",
                newName: "IX_EnvioConvite_IdConvidado");

            migrationBuilder.RenameColumn(
                name: "IdParticipante",
                table: "ContribuicaoPresente",
                newName: "IdConvidado");

            migrationBuilder.RenameIndex(
                name: "IX_ContribuicaoPresente_IdParticipante",
                table: "ContribuicaoPresente",
                newName: "IX_ContribuicaoPresente_IdConvidado");

            migrationBuilder.RenameColumn(
                name: "IdParticipante",
                table: "Acompanhantes",
                newName: "IdConvite");

            migrationBuilder.RenameIndex(
                name: "IX_Acompanhantes_IdParticipante",
                table: "Acompanhantes",
                newName: "IX_Acompanhantes_IdConvite");

            migrationBuilder.CreateTable(
                name: "Convidados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convidados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convidados_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Convidados_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Convidados_IdEvento",
                table: "Convidados",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Convidados_IdFoto",
                table: "Convidados",
                column: "IdFoto");

            migrationBuilder.AddForeignKey(
                name: "FK_Acompanhantes_Convidados_IdConvite",
                table: "Acompanhantes",
                column: "IdConvite",
                principalTable: "Convidados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContribuicaoPresente_Convidados_IdConvidado",
                table: "ContribuicaoPresente",
                column: "IdConvidado",
                principalTable: "Convidados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnvioConvite_Convidados_IdConvidado",
                table: "EnvioConvite",
                column: "IdConvidado",
                principalTable: "Convidados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
