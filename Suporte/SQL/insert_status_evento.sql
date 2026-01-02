DELETE 
FROM StatusEvento;

INSERT INTO StatusEvento (Id, Descricao)
VALUES (1, 'Ativo'),
  (2, 'Inativo'),
  (3, 'Agendado'),
  (4, 'Cancelado'),
  (5, 'Concluido');


--INSERT STATUS PRESENTE
INSERT INTO StatusPresente (Id, Descricao)
VALUES (1, 'Disponível'),
  (2, 'Reservado'),
  (3, 'Indisponível'),
  (4, 'Finalizado');

--INSERT STATUS CONTRIBUICAO
INSERT INTO StatusContribuicao (Id, Descricao)
VALUES (1, 'Pendente'),
  (2, 'Confirmado'),
  (3, 'Cancelado'),
  (4, 'Em Análise'),
  (5, 'Estornado');