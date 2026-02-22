# 🧪 Testando Filas Redis Streams

## ✅ Pré-requisitos (já feitos)

- ✅ Docker containers rodando (`docker-compose up -d`)
- ✅ API iniciada (`dotnet run` em background)

## 📨 1. Enviar requisições POST

### Opção A: Usando o arquivo MinhaApi.http

1. Abra o arquivo `MinhaApi.http`
2. Clique em **"Send Request"** acima de cada requisição POST
3. Observe a resposta HTTP 201 Created

### Opção B: Usando PowerShell

```powershell
# Lote Premium (TeorFe >= 65, Umidade <= 8)
Invoke-RestMethod -Uri "http://localhost:5247/api/LotesMinerio" -Method POST -ContentType "application/json" -Body '{
  "codigoLote": "MNP-2026-000137",
  "minaOrigem": "Carajás",
  "teorFe": 67.5,
  "umidade": 7.2,
  "toneladas": 1500,
  "status": 0,
  "localizacaoAtual": "Pátio A"
}'

# Lote Padrão (TeorFe >= 60, Umidade <= 10)
Invoke-RestMethod -Uri "http://localhost:5247/api/LotesMinerio" -Method POST -ContentType "application/json" -Body '{
  "codigoLote": "MNP-2026-000138",
  "minaOrigem": "Itabira",
  "teorFe": 62.0,
  "umidade": 9.5,
  "toneladas": 2000,
  "status": 0,
  "localizacaoAtual": "Pátio B"
}'

# Lote Baixa Qualidade
Invoke-RestMethod -Uri "http://localhost:5247/api/LotesMinerio" -Method POST -ContentType "application/json" -Body '{
  "codigoLote": "MNP-2026-000139",
  "minaOrigem": "Minas Centrais",
  "teorFe": 55.0,
  "umidade": 12.0,
  "toneladas": 1200,
  "status": 0,
  "localizacaoAtual": "Pátio C"
}'
```

## 🔍 2. Verificar os logs da API

O terminal onde a API está rodando mostrará logs como:

```
[Producer] Lote MNP-2026-000137 (Id=1) enfileirado no Redis Stream com MessageId=1770738550268-0
[Worker] Consumer Group 'grupo-lotes' criado com sucesso.
[Worker] Processando lote MNP-2026-000137 (Id=1) | Ação=RecalcularClassificacao
[Fila] Lote MNP-2026-000137 (Id=1) -> Ação=RecalcularClassificacao | Classificação=Premium
[Worker] Mensagem 1770738550268-0 processada e ACKada com sucesso.
```

## 🔴 3. Comandos Redis para verificar a fila

### Verificar quantas mensagens existem no stream

```powershell
docker exec -it redis_estudos redis-cli -a redis123 XLEN lotes-stream
```

### Listar as mensagens no stream

```powershell
docker exec -it redis_estudos redis-cli -a redis123 XREAD COUNT 10 STREAMS lotes-stream 0
```

### Verificar informações do Consumer Group

```powershell
docker exec -it redis_estudos redis-cli -a redis123 XINFO GROUPS lotes-stream
```

### Verificar mensagens pendentes (não ACKadas)

```powershell
docker exec -it redis_estudos redis-cli -a redis123 XPENDING lotes-stream grupo-lotes
```

### Verificar a DLQ (Dead Letter Queue)

```powershell
docker exec -it redis_estudos redis-cli -a redis123 XLEN lotes-dlq
docker exec -it redis_estudos redis-cli -a redis123 XREAD COUNT 10 STREAMS lotes-dlq 0
```

## 🧹 4. Limpar as filas

### Deletar o stream principal

```powershell
docker exec -it redis_estudos redis-cli -a redis123 DEL lotes-stream
```

### Deletar a DLQ

```powershell
docker exec -it redis_estudos redis-cli -a redis123 DEL lotes-dlq
```

### Deletar tudo do Redis (cuidado!)

```powershell
docker exec -it redis_estudos redis-cli -a redis123 FLUSHALL
```

## 🎯 5. O que esperar

### Fluxo Normal:

1. **POST** → API recebe e salva no banco
2. **Producer** → Enfileira mensagem no Redis Stream (`XADD`)
3. **Worker** → Consome mensagem (`XREADGROUP`)
4. **Processamento** → Classifica o lote (Premium/Padrão/Baixa)
5. **ACK** → Marca como processada (`XACK`)

### Classificação:

- **Premium**: TeorFe >= 65% E Umidade <= 8%
- **Padrão**: TeorFe >= 60% E Umidade <= 10%
- **Baixa Qualidade**: Demais casos

### Reentrega:

- Mensagens não ACKadas após 10s são reclamadas
- Após 3 tentativas → movidas para DLQ (`lotes-dlq`)

## 🛑 6. Parar a API

Para parar a API, encontre o processo e finalize:

```powershell
# Listar os processos da API
Get-Process -Name "MinhaApi"

# Parar todos os processos MinhaApi
Stop-Process -Name "MinhaApi" -Force
```

## 📊 7. Monitoramento em tempo real

Para ver os logs da API enquanto ela processa:

```powershell
# Ver logs do Redis
docker logs -f redis_estudos

# Ver todos os comandos executados no Redis
docker exec -it redis_estudos redis-cli -a redis123 MONITOR
```
