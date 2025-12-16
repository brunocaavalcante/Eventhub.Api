-- =========================================================
-- RESEED DE MÓDULOS, PERMISSÕES E PERFIL-PERMISSÃO (EVENTHUB)
-- =========================================================
-- Este script limpa os registros padrão e reinsere os módulos e permissões
-- associados. Execute em ambiente de homologação/produção somente se estiver
-- ciente de que perfis/permissões customizados serão removidos.

START TRANSACTION;
SET FOREIGN_KEY_CHECKS = 0;

DELETE FROM PerfilPermissao;
DELETE FROM UsuarioPermissao;
DELETE FROM Permissao;
DELETE FROM Modulo;

SET FOREIGN_KEY_CHECKS = 1;

-- =========================================================
-- CATALOGO DE MÓDULOS (cards da aplicação)
-- =========================================================
INSERT INTO Modulo (Nome, Descricao, Icone, Rota, Ordem)
VALUES
    ('Eventos', 'Planejamento e acompanhamento dos eventos', 'event', '/eventos', 1),
    ('Participantes', 'Gestão da lista de participantes e acompanhantes', 'groups', '/participantes', 2),
    ('Galeria', 'Fotos e memórias compartilhadas do evento', 'photo_library', '/galeria', 3),
    ('Convites', 'Planejamento, envio e acompanhamento dos convites', 'drafts', '/convites', 4),
    ('Programacoes', 'Agenda detalhada do evento e responsáveis', 'schedule', '/programacoes', 5),
    ('Notificacoes', 'Alertas e comunicações relevantes ao evento', 'notifications', '/notificacoes', 6),
    ('Presentes', 'Lista de presentes e contribuições financeiras', 'redeem', '/presentes', 7),
    ('Chat', 'Comunicação em tempo real entre participantes', 'chat', '/chat', 8),
    ('Configuracoes', 'Ajustes e preferências do sistema', 'settings', '/configuracoes', 9),
    ('Convidados', 'Gestão de convidados externos ao evento', 'person_add', '/convidados', 10);

SET @ModuloEventosId = (SELECT Id FROM Modulo WHERE Nome = 'Eventos');
SET @ModuloParticipantesId = (SELECT Id FROM Modulo WHERE Nome = 'Participantes');
SET @ModuloGaleriaId = (SELECT Id FROM Modulo WHERE Nome = 'Galeria');
SET @ModuloConvitesId = (SELECT Id FROM Modulo WHERE Nome = 'Convites');
SET @ModuloProgramacoesId = (SELECT Id FROM Modulo WHERE Nome = 'Programacoes');
SET @ModuloNotificacoesId = (SELECT Id FROM Modulo WHERE Nome = 'Notificacoes');
SET @ModuloPresentesId = (SELECT Id FROM Modulo WHERE Nome = 'Presentes');
SET @ModuloChatId = (SELECT Id FROM Modulo WHERE Nome = 'Chat');
SET @ModuloConfiguracoesId = (SELECT Id FROM Modulo WHERE Nome = 'Configuracoes');
SET @ModuloConvidadosId = (SELECT Id FROM Modulo WHERE Nome = 'Convidados');

-- =========================================================
-- PERMISSÕES DO SISTEMA EVENTHUB (vinculadas aos módulos)
-- =========================================================
INSERT INTO Permissao (Nome, Descricao, Chave, IdModulo)
VALUES
    -- Eventos
    ('Visualizar Eventos', 'Permite visualizar eventos', 'eventos.visualizar', @ModuloEventosId),
    ('Criar Eventos', 'Permite criar novos eventos', 'eventos.criar', @ModuloEventosId),
    ('Editar Eventos', 'Permite editar eventos existentes', 'eventos.editar', @ModuloEventosId),
    ('Excluir Eventos', 'Permite excluir eventos', 'eventos.excluir', @ModuloEventosId),

    -- Participantes
    ('Visualizar Participantes', 'Permite visualizar participantes', 'participantes.visualizar', @ModuloParticipantesId),
    ('Adicionar Participantes', 'Permite adicionar participantes ao evento', 'participantes.adicionar', @ModuloParticipantesId),
    ('Editar Participantes', 'Permite editar participantes', 'participantes.editar', @ModuloParticipantesId),
    ('Remover Participantes', 'Permite remover participantes', 'participantes.remover', @ModuloParticipantesId),

    -- Galeria
    ('Visualizar Galeria', 'Permite visualizar galeria de fotos', 'galeria.visualizar', @ModuloGaleriaId),
    ('Adicionar Fotos', 'Permite adicionar fotos à galeria', 'galeria.adicionar', @ModuloGaleriaId),
    ('Editar Fotos', 'Permite editar fotos da galeria', 'galeria.editar', @ModuloGaleriaId),
    ('Excluir Fotos', 'Permite excluir fotos da galeria', 'galeria.excluir', @ModuloGaleriaId),

    -- Convites
    ('Criar Convites', 'Permite criar convites', 'convites.criar', @ModuloConvitesId),
    ('Enviar Convites', 'Permite enviar convites', 'convites.enviar', @ModuloConvitesId),
    ('Visualizar Envios', 'Permite visualizar envios de convites', 'convites.visualizar', @ModuloConvitesId),

    -- Programações
    ('Visualizar Programações', 'Permite visualizar programações do evento', 'programacoes.visualizar', @ModuloProgramacoesId),
    ('Criar Programações', 'Permite criar programações', 'programacoes.criar', @ModuloProgramacoesId),
    ('Editar Programações', 'Permite editar programações', 'programacoes.editar', @ModuloProgramacoesId),
    ('Excluir Programações', 'Permite excluir programações', 'programacoes.excluir', @ModuloProgramacoesId),

    -- Notificações
    ('Visualizar Notificações', 'Permite visualizar notificações', 'notificacoes.visualizar', @ModuloNotificacoesId),
    ('Enviar Notificações', 'Permite enviar notificações', 'notificacoes.enviar', @ModuloNotificacoesId),

    -- Presentes
    ('Visualizar Presentes', 'Permite visualizar lista de presentes', 'presentes.visualizar', @ModuloPresentesId),
    ('Criar Presentes', 'Permite criar/adicionar presentes à lista', 'presentes.criar', @ModuloPresentesId),
    ('Editar Presentes', 'Permite editar presentes', 'presentes.editar', @ModuloPresentesId),
    ('Excluir Presentes', 'Permite excluir presentes', 'presentes.excluir', @ModuloPresentesId),
    ('Gerenciar Contribuições', 'Permite gerenciar contribuições de presentes', 'presentes.contribuicoes', @ModuloPresentesId),

    -- Chat
    ('Participar do Chat', 'Permite participar de conversas no chat', 'chat.participar', @ModuloChatId),

    -- Configurações
    ('Gerenciar Configurações', 'Permite gerenciar configurações do evento', 'configuracoes.gerenciar', @ModuloConfiguracoesId),

    -- Convidados
    ('Visualizar Convidados', 'Permite visualizar convidados externos', 'convidados.visualizar', @ModuloConvidadosId),
    ('Adicionar Convidados', 'Permite adicionar convidados externos', 'convidados.adicionar', @ModuloConvidadosId),
    ('Editar Convidados', 'Permite editar informações de convidados', 'convidados.editar', @ModuloConvidadosId),
    ('Remover Convidados', 'Permite remover convidados externos', 'convidados.remover', @ModuloConvidadosId);

-- =========================================================
-- VINCULAÇÃO PERFIL-PERMISSÃO (preserva mesma lógica anterior)
-- =========================================================
-- 1. ADMINISTRADOR (ID=1) - Controle Total do Sistema
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 1, Id FROM Permissao;

-- 2. MÚSICO (ID=2) - Acesso à programação musical e visualização
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 2, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'programacoes.visualizar',
        'programacoes.criar',
        'programacoes.editar',
        'participantes.visualizar',
        'galeria.visualizar',
        'galeria.adicionar',
        'notificacoes.visualizar',
        'presentes.visualizar',
        'chat.participar',
        'convidados.visualizar'
    );

-- 3. CERIMONIALISTA (ID=3) - Coordenação geral do evento
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 3, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'eventos.editar',
        'participantes.visualizar',
        'participantes.adicionar',
        'participantes.editar',
        'programacoes.visualizar',
        'programacoes.criar',
        'programacoes.editar',
        'programacoes.excluir',
        'galeria.visualizar',
        'convites.criar',
        'convites.enviar',
        'convites.visualizar',
        'notificacoes.visualizar',
        'notificacoes.enviar',
        'presentes.visualizar',
        'presentes.criar',
        'presentes.editar',
        'presentes.excluir',
        'presentes.contribuicoes',
        'chat.participar',
        'configuracoes.gerenciar',
        'convidados.visualizar',
        'convidados.adicionar',
        'convidados.editar',
        'convidados.remover'
    );

-- 4. FOTÓGRAFO/FILMAGEM (ID=4) - Gestão da galeria
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 4, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'participantes.visualizar',
        'galeria.visualizar',
        'galeria.adicionar',
        'galeria.editar',
        'galeria.excluir',
        'programacoes.visualizar',
        'notificacoes.visualizar',
        'presentes.visualizar',
        'chat.participar'
        'convidados.visualizar'
    );

-- 5. SEGURANÇA (ID=5) - Controle de acesso e participantes
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 5, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'participantes.visualizar',
        'participantes.adicionar',
        'convites.visualizar',
        'notificacoes.visualizar',
        'chat.participar'
        'convidados.visualizar',
        'convidados.adicionar',
        'convidados.editar',
        'convidados.remover'
    );

-- 6. BUFFET/COZINHA (ID=6) - Gestão de alimentação e contagem
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 6, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'participantes.visualizar',
        'programacoes.visualizar',
        'notificacoes.visualizar',
        'chat.participar',
        'convidados.visualizar'
    );

-- 7. COORDENADOR (ID=7) - Gestão completa exceto exclusão de evento
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 7, Id
FROM Permissao
WHERE Chave NOT IN ('eventos.excluir');

-- 8. AJUDANTE (ID=8) - Suporte operacional básico
INSERT INTO PerfilPermissao (IdPerfil, IdPermissao)
SELECT 8, Id
FROM Permissao
WHERE Chave IN (
        'eventos.visualizar',
        'participantes.visualizar',
        'galeria.visualizar',
        'galeria.adicionar',
        'programacoes.visualizar',
        'notificacoes.visualizar',
        'presentes.visualizar',
        'chat.participar',
        'convidados.visualizar'
    );

COMMIT;