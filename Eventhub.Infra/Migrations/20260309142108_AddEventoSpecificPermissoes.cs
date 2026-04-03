using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEventoSpecificPermissoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Inserir permissões de visibilidade para eventos
            migrationBuilder.Sql(@"
                INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
                SELECT 'Visualizar Galeria do Evento', 'Permite visualizar a galeria de fotos do evento', 'galeria.view', Id
                FROM Modulo 
                WHERE Nome = 'Galeria'
                AND NOT EXISTS (SELECT 1 FROM Permissao WHERE Chave = 'galeria.view');

                INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
                SELECT 'Visualizar Chat do Evento', 'Permite visualizar e participar do chat do evento', 'chat.view', Id
                FROM Modulo 
                WHERE Nome = 'Chat'
                AND NOT EXISTS (SELECT 1 FROM Permissao WHERE Chave = 'chat.view');

                INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
                SELECT 'Visualizar Lista de Presentes', 'Permite visualizar a lista de presentes do evento', 'presente.view', Id
                FROM Modulo 
                WHERE Nome = 'Presentes'
                AND NOT EXISTS (SELECT 1 FROM Permissao WHERE Chave = 'presente.view');

                INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
                SELECT 'Visualizar Lista de Convidados', 'Permite visualizar a lista de convidados do evento', 'convidados.view', Id
                FROM Modulo 
                WHERE Nome = 'Participantes'
                AND NOT EXISTS (SELECT 1 FROM Permissao WHERE Chave = 'convidados.view');

                INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
                SELECT 'Visualizar Agenda do Evento', 'Permite visualizar a programação/agenda do evento', 'agenda.view', Id
                FROM Modulo 
                WHERE Nome = 'Programacoes'
                AND NOT EXISTS (SELECT 1 FROM Permissao WHERE Chave = 'agenda.view');
            ");

            migrationBuilder.CreateTable(
                name: "ParticipantePermissao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdParticipante = table.Column<int>(type: "int", nullable: false),
                    IdPermissao = table.Column<int>(type: "int", nullable: false),
                    Concedida = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantePermissao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantePermissao_Participante_IdParticipante",
                        column: x => x.IdParticipante,
                        principalTable: "Participante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantePermissao_Permissao_IdPermissao",
                        column: x => x.IdPermissao,
                        principalTable: "Permissao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PerfilEventoPermissao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerfil = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdPermissao = table.Column<int>(type: "int", nullable: false),
                    Concedida = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilEventoPermissao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilEventoPermissao_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilEventoPermissao_Perfil_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerfilEventoPermissao_Permissao_IdPermissao",
                        column: x => x.IdPermissao,
                        principalTable: "Permissao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantePermissao_IdParticipante_IdPermissao",
                table: "ParticipantePermissao",
                columns: new[] { "IdParticipante", "IdPermissao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantePermissao_IdPermissao",
                table: "ParticipantePermissao",
                column: "IdPermissao");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilEventoPermissao_IdEvento",
                table: "PerfilEventoPermissao",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilEventoPermissao_IdPerfil_IdEvento_IdPermissao",
                table: "PerfilEventoPermissao",
                columns: new[] { "IdPerfil", "IdEvento", "IdPermissao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilEventoPermissao_IdPermissao",
                table: "PerfilEventoPermissao",
                column: "IdPermissao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantePermissao");

            migrationBuilder.DropTable(
                name: "PerfilEventoPermissao");

            // Remover permissões de visibilidade
            migrationBuilder.Sql(@"
                DELETE FROM Permissao WHERE Chave IN (
                    'galeria.view',
                    'chat.view',
                    'presente.view',
                    'convidados.view',
                    'agenda.view'
                );
            ");
        }
    }
}
