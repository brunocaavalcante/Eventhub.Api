using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventhub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EnderecoEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Numero = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logradouro = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cep = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PontoReferencia = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnderecoEvento", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataUpload = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TamanhoKB = table.Column<int>(type: "int", nullable: false),
                    Base64 = table.Column<string>(type: "LONGTEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StatusContribuicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusContribuicao", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StatusEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEvento", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StatusPresente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPresente", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StatusProgramacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusProgramacao", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Foto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Convite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mensagem = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemaConvite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdFoto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convite_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificacaoTipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoPadrao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconePadrao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FotosId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacaoTipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacaoTipo_Fotos_FotosId",
                        column: x => x.FotosId,
                        principalTable: "Fotos",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TipoEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoEvento_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComentarioFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Comentario = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentarioFoto_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentarioFoto_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CurtidaFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurtidaFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurtidaFoto_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurtidaFoto_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdTipoEvento = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IdEndereco = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCriador = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaxConvidado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evento_EnderecoEvento_IdEndereco",
                        column: x => x.IdEndereco,
                        principalTable: "EnderecoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_StatusEvento_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "StatusEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_TipoEvento_IdTipoEvento",
                        column: x => x.IdTipoEvento,
                        principalTable: "TipoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_Usuario_IdUsuarioCriador",
                        column: x => x.IdUsuarioCriador,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Convidados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Galeria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Visibilidade = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Legenda = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galeria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galeria_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Galeria_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notificacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioOrigem = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioDestino = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkAcao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotificacaoTipoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacao_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificacao_NotificacaoTipo_NotificacaoTipoId",
                        column: x => x.NotificacaoTipoId,
                        principalTable: "NotificacaoTipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificacao_Usuario_IdUsuarioDestino",
                        column: x => x.IdUsuarioDestino,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificacao_Usuario_IdUsuarioOrigem",
                        column: x => x.IdUsuarioOrigem,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Presente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LinkProduto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presente_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presente_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presente_StatusPresente_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "StatusPresente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProgramacaoEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Duracao = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Local = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdFoto = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Responsavel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacaoEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramacaoEvento_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramacaoEvento_Fotos_IdFoto",
                        column: x => x.IdFoto,
                        principalTable: "Fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramacaoEvento_StatusProgramacao_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "StatusProgramacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsuarioPerfilEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdPerfil = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPerfilEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPerfilEvento_Evento_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioPerfilEvento_Perfil_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioPerfilEvento_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Acompanhantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdConvite = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acompanhantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acompanhantes_Convidados_IdConvite",
                        column: x => x.IdConvite,
                        principalTable: "Convidados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EnvioConvite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdConvite = table.Column<int>(type: "int", nullable: false),
                    IdConvidado = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MensagemResposta = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QtdAcompanhantes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioConvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvioConvite_Convidados_IdConvidado",
                        column: x => x.IdConvidado,
                        principalTable: "Convidados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContribuicaoPresente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPresente = table.Column<int>(type: "int", nullable: false),
                    IdConvidado = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LinkProduto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FormaPagamento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StatusContribuicaoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContribuicaoPresente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContribuicaoPresente_Convidados_IdConvidado",
                        column: x => x.IdConvidado,
                        principalTable: "Convidados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContribuicaoPresente_Presente_IdPresente",
                        column: x => x.IdPresente,
                        principalTable: "Presente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContribuicaoPresente_StatusContribuicao_StatusContribuicaoId",
                        column: x => x.StatusContribuicaoId,
                        principalTable: "StatusContribuicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResponsavelProgramacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdProgramacao = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Funcao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsavelProgramacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsavelProgramacao_ProgramacaoEvento_IdProgramacao",
                        column: x => x.IdProgramacao,
                        principalTable: "ProgramacaoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResponsavelProgramacao_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Acompanhantes_IdConvite",
                table: "Acompanhantes",
                column: "IdConvite");

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioFoto_IdFoto",
                table: "ComentarioFoto",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioFoto_IdUsuario",
                table: "ComentarioFoto",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ContribuicaoPresente_IdConvidado",
                table: "ContribuicaoPresente",
                column: "IdConvidado");

            migrationBuilder.CreateIndex(
                name: "IX_ContribuicaoPresente_IdPresente",
                table: "ContribuicaoPresente",
                column: "IdPresente");

            migrationBuilder.CreateIndex(
                name: "IX_ContribuicaoPresente_StatusContribuicaoId",
                table: "ContribuicaoPresente",
                column: "StatusContribuicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Convidados_IdEvento",
                table: "Convidados",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Convidados_IdFoto",
                table: "Convidados",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_Convite_IdFoto",
                table: "Convite",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_CurtidaFoto_IdFoto_IdUsuario",
                table: "CurtidaFoto",
                columns: new[] { "IdFoto", "IdUsuario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurtidaFoto_IdUsuario",
                table: "CurtidaFoto",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdConvidado",
                table: "EnvioConvite",
                column: "IdConvidado");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdConvite",
                table: "EnvioConvite",
                column: "IdConvite");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioConvite_IdEvento",
                table: "EnvioConvite",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_IdEndereco",
                table: "Evento",
                column: "IdEndereco");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_IdStatus",
                table: "Evento",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_IdTipoEvento",
                table: "Evento",
                column: "IdTipoEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_IdUsuarioCriador",
                table: "Evento",
                column: "IdUsuarioCriador");

            migrationBuilder.CreateIndex(
                name: "IX_EventoFoto_IdFoto",
                table: "EventoFoto",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_Galeria_IdEvento",
                table: "Galeria",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Galeria_IdFoto",
                table: "Galeria",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdEvento",
                table: "Notificacao",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdUsuarioDestino",
                table: "Notificacao",
                column: "IdUsuarioDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdUsuarioOrigem",
                table: "Notificacao",
                column: "IdUsuarioOrigem");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_NotificacaoTipoId",
                table: "Notificacao",
                column: "NotificacaoTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoTipo_FotosId",
                table: "NotificacaoTipo",
                column: "FotosId");

            migrationBuilder.CreateIndex(
                name: "IX_Presente_IdEvento",
                table: "Presente",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Presente_IdFoto",
                table: "Presente",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_Presente_IdStatus",
                table: "Presente",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacaoEvento_IdEvento",
                table: "ProgramacaoEvento",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacaoEvento_IdFoto",
                table: "ProgramacaoEvento",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacaoEvento_IdStatus",
                table: "ProgramacaoEvento",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsavelProgramacao_IdProgramacao",
                table: "ResponsavelProgramacao",
                column: "IdProgramacao");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsavelProgramacao_IdUsuario",
                table: "ResponsavelProgramacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_TipoEvento_IdFoto",
                table: "TipoEvento",
                column: "IdFoto");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfilEvento_IdEvento_IdUsuario_IdPerfil",
                table: "UsuarioPerfilEvento",
                columns: new[] { "IdEvento", "IdUsuario", "IdPerfil" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfilEvento_IdPerfil",
                table: "UsuarioPerfilEvento",
                column: "IdPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfilEvento_IdUsuario",
                table: "UsuarioPerfilEvento",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acompanhantes");

            migrationBuilder.DropTable(
                name: "ComentarioFoto");

            migrationBuilder.DropTable(
                name: "ContribuicaoPresente");

            migrationBuilder.DropTable(
                name: "CurtidaFoto");

            migrationBuilder.DropTable(
                name: "EnvioConvite");

            migrationBuilder.DropTable(
                name: "EventoFoto");

            migrationBuilder.DropTable(
                name: "Galeria");

            migrationBuilder.DropTable(
                name: "Notificacao");

            migrationBuilder.DropTable(
                name: "ResponsavelProgramacao");

            migrationBuilder.DropTable(
                name: "UsuarioPerfilEvento");

            migrationBuilder.DropTable(
                name: "Presente");

            migrationBuilder.DropTable(
                name: "StatusContribuicao");

            migrationBuilder.DropTable(
                name: "Convidados");

            migrationBuilder.DropTable(
                name: "Convite");

            migrationBuilder.DropTable(
                name: "NotificacaoTipo");

            migrationBuilder.DropTable(
                name: "ProgramacaoEvento");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropTable(
                name: "StatusPresente");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "StatusProgramacao");

            migrationBuilder.DropTable(
                name: "EnderecoEvento");

            migrationBuilder.DropTable(
                name: "StatusEvento");

            migrationBuilder.DropTable(
                name: "TipoEvento");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Fotos");
        }
    }
}
