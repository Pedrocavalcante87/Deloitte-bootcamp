# 🏭 Sistema de Gestão de Lotes de Minério

API REST completa para gerenciamento de lotes de minério de ferro com processamento assíncrono, cache distribuído e análises de qualidade em tempo real.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Como Executar](#como-executar)
- [Testes](#testes)
- [Filas Redis Streams](#filas-redis-streams)
- [Monitoramento](#monitoramento)
- [Estrutura do Projeto](#estrutura-do-projeto)

---

## 🎯 Sobre o Projeto

Sistema empresarial para controle de lotes de minério de ferro, implementando:

- ✅ **CRUD completo** de lotes com validações de negócio
- ✅ **Processamento assíncrono** com Redis Streams
- ✅ **Cache distribuído** para otimização de consultas
- ✅ **Classificação automática** de qualidade (Premium/Padrão/Baixa)
- ✅ **Controle logístico** com rastreamento de movimentação
- ✅ **Cálculos de precificação** baseados em qualidade
- ✅ **Resiliência** com reentrega automática e DLQ (Dead Letter Queue)

---

## 🚀 Tecnologias

| Tecnologia                | Versão   | Uso                        |
| ------------------------- | -------- | -------------------------- |
| **.NET**                  | 10.0     | Framework principal        |
| **PostgreSQL**            | 16       | Banco de dados relacional  |
| **Redis**                 | 7-alpine | Cache + Filas (Streams)    |
| **Entity Framework Core** | 10.0     | ORM com Snake Case         |
| **StackExchange.Redis**   | 2.10.14  | Cliente Redis              |
| **Docker Compose**        | -        | Orquestração de containers |
| **xUnit**                 | -        | Framework de testes        |
| **Testcontainers**        | -        | Testes de integração       |

---

## 🏗️ Arquitetura

### Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    Controllers (HTTP)                        │
│  LotesMinerioController, LotesExtrasController              │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────┴────────────────────────────────────────────┐
│                  Services (Lógica de Negócio)               │
│  LoteService, LoteQueueProducer, RedisCacheService          │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────┴────────────────────────────────────────────┐
│                  Data (Persistência)                         │
│  AppDbContext (EF Core), PostgreSQL                         │
└─────────────────────────────────────────────────────────────┘
```

### Processamento Assíncrono (Redis Streams)

```
┌──────────────┐    ┌───────────────┐    ┌─────────────────┐
│  HTTP POST   │───▶│   Producer    │───▶│  Redis Stream   │
│ LotesMinerio │    │  (XADD)       │    │  lotes-stream   │
└──────────────┘    └───────────────┘    └────────┬────────┘
                                                   │
                    ┌──────────────────────────────┘
                    │
                    ▼
         ┌────────────────────┐
         │  LoteQueueWorker   │
         │  (BackgroundService)│
         └──────────┬──────────┘
                    │
         ┌──────────┴──────────┐
         │  XREADGROUP + XACK  │
         │  • Processa lote    │
         │  • Classifica       │
         │  • ACK ou DLQ       │
         └─────────────────────┘
```

---

## 🎯 Funcionalidades

### 1. Gestão de Lotes (CRUD)

#### Criar Lote

```http
POST /api/LotesMinerio
Content-Type: application/json

{
  "codigoLote": "MNP-2026-000137",
  "minaOrigem": "Carajás",
  "teorFe": 67.5,
  "umidade": 7.2,
  "siO2": 1.8,
  "p": 0.05,
  "toneladas": 1500,
  "status": 0,
  "localizacaoAtual": "Pátio A"
}
```

#### Listar Lotes (com paginação e filtros)

```http
GET /api/LotesMinerio?page=1&pageSize=10&status=0&mina=Carajás
```

#### Buscar por ID (com cache)

```http
GET /api/LotesMinerio/1
```

### 2. Cálculos de Negócio

#### Classificação de Qualidade

```http
GET /api/lotes/1/classificacao
```

**Resposta:**

```json
{
  "loteId": 1,
  "codigoLote": "MNP-2026-000137",
  "classificacao": "Premium",
  "criterios": {
    "teorFe": 67.5,
    "umidade": 7.2,
    "siO2": 1.8
  }
}
```

**Regras:**

- **Premium**: TeorFe ≥ 65% E Umidade ≤ 8%
- **Padrão**: TeorFe ≥ 60% E Umidade ≤ 10%
- **Baixa Qualidade**: Demais casos

#### Preço por Tonelada

```http
GET /api/lotes/1/preco
```

#### Penalidade por Umidade

```http
GET /api/lotes/1/penalidade-umidade
```

### 3. Controle Logístico

#### Registrar Movimentação

```http
POST /api/lotes/1/movimentacao?localDestino=Porto de Tubarão
```

#### Avançar Status do Lote

```http
POST /api/lotes/1/avancar-status
```

**Fluxo:** EmEstoque → EmTransporte → Embarcado

---

## 🏃 Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Subir infraestrutura (PostgreSQL + Redis)

```powershell
cd GestaoLotesMinerio/MinhaApi
docker-compose up -d
```

Aguarde até os containers estarem "healthy":

```powershell
docker ps
```

### 2. Executar a API

```powershell
dotnet run
```

A API estará disponível em: **http://localhost:5247**

### 3. Testar endpoints

Use o arquivo [MinhaApi.http](MinhaApi/MinhaApi.http) no VS Code com a extensão REST Client, ou:

```powershell
# Criar um lote
Invoke-RestMethod -Uri "http://localhost:5247/api/LotesMinerio" -Method POST -ContentType "application/json" -InFile teste-lote.json

# Listar lotes
Invoke-RestMethod -Uri "http://localhost:5247/api/LotesMinerio" -Method GET
```

---

## 🧪 Testes

### Executar todos os testes

```powershell
cd GestaoLotesMinerio/MinhaApi.tests
dotnet test --verbosity normal
```

### Cobertura de Testes

- ✅ **Testes Unitários**: DTOs, Models, validações
- ✅ **Testes de Integração**: Controllers com Testcontainers (PostgreSQL real)
- ✅ **Scenarios**: Fluxos completos de negócio

### Estrutura de Testes

```
MinhaApi.tests/
├── Controllers/        # Testes de integração dos controllers
├── Dtos/              # Validações de DTOs
├── Models/            # Lógica de domínio
├── Fixtures/          # Setup de testes
└── Scenarios/         # Cenários de negócio
```

---

## 🔄 Filas Redis Streams

### Arquitetura de Filas

O sistema utiliza **Redis Streams** para processamento assíncrono de lotes.

#### Componentes

1. **Producer** (`LoteQueueProducer`)
   - Serializa mensagem em JSON
   - Executa `XADD` no stream `lotes-stream`
   - Retorna o MessageId gerado

2. **Worker** (`LoteQueueWorker`)
   - BackgroundService que roda continuamente
   - Garante Consumer Group (`XGROUP CREATE`)
   - Lê mensagens com `XREADGROUP`
   - Processa e classifica o lote
   - Faz `XACK` quando bem-sucedido
   - Reenfileira com `XAUTOCLAIM` (idle > 10s)
   - Move para DLQ após 3 tentativas

3. **DLQ (Dead Letter Queue)**
   - Stream: `lotes-dlq`
   - Armazena mensagens que falharam 3+ vezes
   - Inclui metadata (motivo, timestamp, dados originais)

### Fluxo de Processamento

```
POST /api/LotesMinerio
        ↓
Salva no PostgreSQL
        ↓
Producer.EnfileirarAsync()
        ↓
Redis Stream (XADD)
        ↓
Worker (XREADGROUP)
        ↓
Classifica lote (Premium/Padrão/Baixa)
        ↓
        ├─ Sucesso → XACK
        └─ Erro → Reentrega (max 3x) → DLQ
```

### Comandos Redis Úteis

```powershell
# Ver tamanho da fila
docker exec -it redis_estudos redis-cli -a redis123 XLEN lotes-stream

# Listar mensagens
docker exec -it redis_estudos redis-cli -a redis123 XREAD COUNT 10 STREAMS lotes-stream 0

# Ver consumer groups
docker exec -it redis_estudos redis-cli -a redis123 XINFO GROUPS lotes-stream

# Ver mensagens pendentes
docker exec -it redis_estudos redis-cli -a redis123 XPENDING lotes-stream grupo-lotes

# Limpar fila
docker exec -it redis_estudos redis-cli -a redis123 DEL lotes-stream

# Ver DLQ
docker exec -it redis_estudos redis-cli -a redis123 XREAD COUNT 10 STREAMS lotes-dlq 0
```

### Configuração

**appsettings.json**

```json
{
  "Redis": {
    "Configuration": "localhost:6379",
    "Password": "redis123",
    "AbortOnConnectFail": false,
    "ConnectTimeout": 5000,
    "ConnectRetry": 3
  }
}
```

---

## 📊 Monitoramento

### Logs da Aplicação

Os logs mostram todo o ciclo de vida das mensagens:

```
[Producer] Lote MNP-2026-000137 (Id=5) enfileirado no Redis Stream com MessageId=1771773031611-0
[Worker] Consumer Group 'grupo-lotes' criado com sucesso.
[Fila] Processando lote MNP-2026-000137 (Id=5) | Ação=RecalcularClassificacao
[Fila] Lote MNP-2026-000137 (Id=5) -> Ação=RecalcularClassificacao | Classificação=Premium
[Worker] Mensagem 1771773031611-0 processada e ACKada com sucesso.
```

### Monitorar Redis em tempo real

```powershell
# Ver todos os comandos executados
docker exec -it redis_estudos redis-cli -a redis123 MONITOR

# Logs do container Redis
docker logs -f redis_estudos

# Logs do container PostgreSQL
docker logs -f pg_estudos
```

### Health Checks

Os containers têm health checks configurados:

```yaml
healthcheck:
  test: ["CMD", "redis-cli", "--raw", "incr", "ping"]
  interval: 5s
  timeout: 3s
  retries: 5
```

---

## 📁 Estrutura do Projeto

```
GestaoLotesMinerio/
├── MinhaApi/                          # API Principal
│   ├── Controllers/                   # Endpoints HTTP
│   │   ├── LotesMinerioController.cs  # CRUD de lotes
│   │   └── LotesExtrasController.cs   # Endpoints de negócio
│   ├── Services/                      # Lógica de negócio
│   │   ├── ILoteQueueProducer.cs     # Interface do Producer
│   │   ├── LoteQueueProducer.cs      # Implementação Producer
│   │   ├── LoteQueueWorker.cs        # Worker (Consumer)
│   │   ├── ICacheService.cs          # Interface Cache
│   │   ├── RedisCacheService.cs      # Implementação Cache
│   │   ├── ILoteService.cs           # Interface Serviços
│   │   └── LoteService.cs            # Cálculos de negócio
│   ├── Data/                          # Camada de dados
│   │   └── AppDbContext.cs           # Context EF Core
│   ├── Models/                        # Entidades
│   │   └── LoteMinerio.cs            # Modelo principal
│   ├── Dtos/                          # Data Transfer Objects
│   │   ├── ProcessarLoteMessage.cs   # DTO da fila
│   │   ├── CreateLoteMinerioDto.cs
│   │   ├── UpdateLoteMinerioDto.cs
│   │   └── ...
│   ├── Program.cs                     # Configuração da aplicação
│   ├── appsettings.json              # Configurações
│   ├── docker-compose.yml            # PostgreSQL + Redis
│   ├── MinhaApi.http                 # Exemplos de requisições
│   └── TESTE_FILAS_REDIS.md          # Guia de testes
│
├── MinhaApi.tests/                    # Testes
│   ├── Controllers/                   # Testes de integração
│   ├── Dtos/                          # Testes unitários DTOs
│   ├── Models/                        # Testes unitários Models
│   ├── Fixtures/                      # Setup de testes
│   └── Scenarios/                     # Cenários completos
│
└── README.md                          # Este arquivo
```

---

## 🔧 Configurações Importantes

### Variáveis de Ambiente (Docker)

```bash
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=minhaapi_db
POSTGRES_PORT=5433
REDIS_PORT=6379
REDIS_PASSWORD=redis123
```

### Connection Strings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=minhaapi_db;Username=postgres;Password=postgres"
  }
}
```

---

## 📚 Recursos Adicionais

- [TESTE_FILAS_REDIS.md](MinhaApi/TESTE_FILAS_REDIS.md) - Guia completo de testes
- [REDIS_CONFIG.md](MinhaApi/REDIS_CONFIG.md) - Configuração detalhada do Redis
- [LOTES_EXTRAS_ENDPOINTS.md](MinhaApi/LOTES_EXTRAS_ENDPOINTS.md) - Documentação endpoints extras
- [MinhaApi.http](MinhaApi/MinhaApi.http) - Exemplos de requisições HTTP
- [LotesExtras.http](MinhaApi/LotesExtras.http) - Requisições endpoints extras

---

## 🎓 Conceitos Implementados

### Padrões de Projeto

- ✅ Repository Pattern (via EF Core)
- ✅ Dependency Injection
- ✅ Producer-Consumer Pattern
- ✅ Cache-Aside Pattern

### Práticas de Engenharia

- ✅ Separação de camadas (Controllers, Services, Data)
- ✅ DTOs para input/output
- ✅ Validações centralizadas
- ✅ Testes automatizados
- ✅ Health checks
- ✅ Logs estruturados
- ✅ Tratamento de erros

### Resiliência

- ✅ Reentrega automática de mensagens
- ✅ Dead Letter Queue (DLQ)
- ✅ Circuit breaker no Redis (AbortOnConnectFail=false)
- ✅ Timeouts configuráveis
- ✅ Consumer Groups para escalabilidade

---

## 👥 Autor

**Bootcamp Deloitte - Fevereiro 2026**

Sistema desenvolvido como projeto prático de API REST com .NET, demonstrando integração completa com bancos de dados relacionais, cache distribuído e processamento assíncrono em arquitetura enterprise.

---

## 📄 Licença

Projeto educacional - Bootcamp Deloitte
