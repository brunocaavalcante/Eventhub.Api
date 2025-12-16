INSERT INTO Fotos (Id, NomeArquivo, Base64, DataUpload, TamanhoKB)
VALUES (1, 'sem-foto.png', '', NOW(), 0);

INSERT INTO TipoEvento (Id, Icon, Nome, Descricao, IdFoto)
VALUES
(1, 'home', 'Chá de Casa Nova', 'Comemore seu novo lar.', 1),
(2, 'favorite', 'Casamento', 'Celebre o amor e a união.', 1),
(3, 'cake', 'Aniversário', 'Marque mais um ano de vida.', 1),
(4, 'child_friendly', 'Chá de Bebê', 'Dê boas-vindas ao bebê.', 1),
(5, 'school', 'Formatura', 'Homenageie uma conquista.', 1),
(6, 'groups', 'Networking', 'Conecte-se com pessoas.', 1),
(7, 'business_center', 'Corporativo', 'Celebre um marco profissional.', 1),
(8, 'apps', 'Outro', 'Crie um evento personalizado.', 1);