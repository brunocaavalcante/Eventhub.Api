DELETE 
FROM StatusEvento;

INSERT INTO StatusEvento (Id, Descricao)
VALUES (1, 'Ativo'),
  (2, 'Inativo'),
  (3, 'Agendado'),
  (4, 'Cancelado'),
  (5, 'Concluido');