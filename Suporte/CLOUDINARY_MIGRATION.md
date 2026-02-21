# Migração de Base64 para Cloudinary

## Visão Geral

Este documento descreve o processo de migração de imagens armazenadas em Base64 no banco de dados para o Cloudinary, um serviço de armazenamento em nuvem.

## Arquivos Criados

### 1. Migration SQL: `20260218_MigrateBase64ToCloudinary.cs`
- Localização: `Eventhub.Infra/Migrations/`
- Responsabilidade: Criar uma entrada de migração no histórico do Entity Framework
- Status: Placeholder para a migração de dados

### 2. Serviço de Migração: `CloudinaryMigrationService.cs`
- Localização: `Eventhub.Infra/Services/`
- Responsabilidade: Orquestrar a migração de fotos
- Funcionalidades:
  - Busca fotos sem URL (com Base64)
  - Faz upload para Cloudinary
  - Atualiza banco de dados com URL e PublicId

### 3. Controller de Migração: `MigrationController.cs`
- Localização: `Eventhub.Api/Controllers/`
- Endpoints:
  - `POST /api/migration/migrate-base64-to-cloudinary` - Inicia a migração
  - `GET /api/migration/migration-status` - Verifica status da migração
- Segurança: Requer autenticação e role "Admin"

## Como Usar

### Passo 1: Aplicar a Migration
```bash
dotnet ef database update
```

### Passo 2: Executar a Migração de Dados
Use a API para iniciar a migração:

```bash
curl -X POST "https://seu-api.com/api/migration/migrate-base64-to-cloudinary" \
     -H "Authorization: Bearer {seu-token-jwt}"
```

Ou no VS Code, use a extensão REST Client:

```http
POST https://seu-api.com/api/migration/migrate-base64-to-cloudinary
Authorization: Bearer {seu-token-jwt}
```

### Passo 3: Verificar Status
```bash
curl -X GET "https://seu-api.com/api/migration/migration-status" \
     -H "Authorization: Bearer {seu-token-jwt}"
```

## Configuração Necessária

Certifique-se de que o Cloudinary está configurado em `appsettings.json`:

```json
{
  "Cloudinary": {
    "CloudName": "seu-cloud-name",
    "ApiKey": "sua-chave-api",
    "ApiSecret": "seu-segredo-api"
  }
}
```

## Pré-requisitos

1. ✓ Já existe configuração do Cloudinary no projeto
2. ✓ Tabela `Fotos` tem colunas: `Url`, `PublicId`, `ContentType`
3. ✓ O usuário executando tem role "Admin"

## Fluxo de Migração

```
┌─────────────────────────────────────┐
│  Buscar fotos do banco de dados     │
│  (onde Url é vazio/null)            │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Para cada foto:                    │
│  - Decodificar Base64               │
│  - Fazer upload para Cloudinary     │
│  - Obter URL e PublicId             │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Atualizar banco de dados           │
│  - Salvar URL                       │
│  - Salvar PublicId                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Log de sucesso/erro                │
│  Relatório de migração              │
└─────────────────────────────────────┘
```

## Logs Esperados

Sucesso:
```
[Information] Foto 1 (foto-casamento.jpg) migrada com sucesso. PublicId: eventhub/abc123_foto-casamento.jpg
[Information] Migração concluída. Sucessos: 100, Falhas: 0
```

Erro:
```
[Error] Erro ao migrar foto 5: Arquivo não encontrado
[Error] Erro na migração: Timeout ao fazer upload
```

## Considerações Importantes

⚠️ **Backup**: Fazer backup do banco de dados antes de executar a migração

⚠️ **Quotas**: Verificar quota do Cloudinary antes de executar

⚠️ **Performance**: A migração pode levar tempo dependendo da quantidade de fotos

⚠️ **Rollback**: Em caso de erro, a migração pode ser reexecutada

## Estrutura da Tabela Fotos Após Migração

```sql
CREATE TABLE Fotos (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    NomeArquivo VARCHAR(255),
    DataUpload DATETIME,
    TamanhoKB INT,
    Url VARCHAR(2048),              -- ✓ Armazena URL do Cloudinary
    PublicId VARCHAR(255),           -- ✓ ID único no Cloudinary
    ContentType VARCHAR(255),        -- ✓ Tipo MIME
    -- Relacionamentos...
);
```

## Próximos Passos (Futuro)

1. Implementar limpeza automática de Base64 antigo
2. Criar job scheduler para migração automática de novos uploads
3. Implementar sincronização bidirecional com Cloudinary
4. Adicionar versionamento de imagens

## Troubleshooting

### Erro: "Arquivo não encontrado"
- Verificar se o arquivo existe no diretório de upload
- Verificar permissões de leitura do arquivo

### Erro: "Timeout ao fazer upload"
- Aumentar timeout nas configurações
- Verificar conexão com internet
- Verificar quota do Cloudinary

### Erro: "Unauthorized"
- Verificar credenciais do Cloudinary
- Verificar permissões de upload

## Contato

Para dúvidas ou problemas na migração, abra uma issue no repositório.
