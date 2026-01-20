# User Story: Contribuição de Presente

**ID:** US-002  
**Título:** Contribuição de Presente com Comprovante  
**Status:** Implementado  
**Versão:** 1.0

---

## 1. Objetivo

**Como** convidado/participante,  
**Eu quero** registrar uma contribuição para um presente com comprovante de pagamento (PIX),  
**Para que** o organizador possa analisar e confirmar o recebimento.

---

## 2. Contexto

Os presentes podem ser financiados por contribuições parciais. Cada contribuição exige um comprovante (imagem) e inicia com status **Em análise**. O status do presente deve refletir o total arrecadado.

---

## 3. Regras de Negócio

1. **Comprovante obrigatório:** toda contribuição deve ter imagem anexada.
2. **Status da contribuição:** inicia como **Em análise**.
3. **Status do presente:**
   - Se soma das contribuições atingir o valor do presente → **Reservado**.
   - Caso contrário → **Em arrecadação**.
4. **Uma foto por contribuição:** a imagem é exclusiva da contribuição.

---

## 4. Critérios de Aceitação

### Cadastro de contribuição
- ✓ É possível cadastrar contribuição informando: IdPresente, IdParticipante, Valor, FormaPagamento e Comprovante.
- ✓ O sistema salva a contribuição e o comprovante.
- ✓ Status da contribuição é **Em análise**.
- ✓ Status do presente é atualizado conforme a soma das contribuições.

### Retorno
- ✓ Retorna os dados da contribuição cadastrada (sem foto no response).

---

## 5. Fluxo Principal

1. Participante informa os dados da contribuição e envia o comprovante.
2. Sistema valida os dados e faz upload do comprovante.
3. Contribuição é salva com status **Em análise**.
4. Sistema recalcula o total arrecadado do presente.
5. Presente passa para **Reservado** se total da contribuição atingir o valor; caso contrário, **Em arrecadação**.

---

## 6. Validações

| Campo | Regra |
|-------|-------|
| IdPresente | Obrigatório, > 0 |
| IdParticipante | Obrigatório, > 0 |
| Valor | Obrigatório, > 0 |
| FormaPagamento | Obrigatório, máx. 100 caracteres |
| Comprovante | Obrigatório, Base64 válido, extensão .jpg/.jpeg/.png |

---

## 7. Mensagens do Sistema

| Situação | Mensagem |
|----------|----------|
| Sucesso | "Contribuição cadastrada com sucesso." |
| Presente não encontrado | "Presente não encontrado." |
| Validação | "O valor da contribuição deve ser maior que 0." |
| Comprovante inválido | "Arquivo não está em Base64 válido." |

---

## 8. Endpoints

- **POST** /api/Presentes/contribuicoes
  - Body: CreateContribuicaoPresenteDto
  - Retorno: ContribuicaoPresenteDto

---

## 9. Histórico de Versões

| Versão | Data | Descrição |
|--------|------|-----------|
| 1.0 | 19/01/2026 | Criação da história de contribuição de presente |
