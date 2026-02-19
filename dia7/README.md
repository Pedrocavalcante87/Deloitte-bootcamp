# Dia 7 - API REST para Lotes de Minério

API para gerenciar lotes de minério de ferro com informações de qualidade, localização, status logístico e cálculos de negócio. Integra PostgreSQL, Redis e suite completa de testes.

## Como executar

```powershell
# Subir PostgreSQL e Redis
cd dia7/MinhaApi
docker-compose up -d

# Rodar a API
dotnet run
# API disponível em http://localhost:5000

# Executar testes
cd ../MinhaApi.tests
dotnet test
```

## Funcionalidades

### Gestão de Lotes

- Criar, consultar, atualizar e deletar lotes de minério
- Consultas otimizadas com cache Redis (TTL 5 minutos)
- Listagem completa dos lotes cadastrados

### Cálculos de Negócio

- **Classificação de qualidade:** Analisa teor de ferro e contaminantes (Premium, Padrão ou Baixa)
- **Precificação:** Calcula preço por tonelada baseado na qualidade do lote
- **Penalidade de umidade:** Calcula descontos por umidade acima do padrão
- **Movimentação logística:** Atualiza status e localização com registro de histórico

### Exemplo de uso

```json
POST /api/lotesminerio
{
  "codigoLote": "MNA-2026-000123",
  "minaOrigem": "Carajás N4E",
  "teorFe": 68.5,
  "umidade": 7.2,
  "siO2": 1.8,
  "p": 0.05,
  "toneladas": 1500,
  "status": 0,
  "localizacaoAtual": "Pátio Carajás"
}
```

## Implementação

**Arquitetura:** Controllers → Services → Data, com separação clara de responsabilidades.

**Cache distribuído:** Consultas frequentes armazenadas no Redis, reduzindo carga no banco.

**Testes automatizados:** Unitários para DTOs/Models e integração com Testcontainers (PostgreSQL real).

**Validações:** Código único, percentuais válidos (0-100), status controlado, regras de mineração aplicadas.

---

**Bootcamp Deloitte - Fevereiro 2026**
