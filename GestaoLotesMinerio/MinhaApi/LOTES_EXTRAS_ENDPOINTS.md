# Documentação - Endpoints Extras de Lotes

## Rotas Implementadas

Todas as rotas estão disponíveis em: `http://localhost:5000/api/lotes`

---

## 📋 Métodos GET (sem body)

### 1. Classificação de Qualidade

**Endpoint:** `GET /api/lotes/{id}/classificacao`

**Exemplo:** `GET http://localhost:5000/api/lotes/1/classificacao`

**Resposta:**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "classificacao": "Premium",
  "teorFe": 68.5,
  "umidade": 7.2,
  "siO2": 1.8,
  "observacao": "Lote de alta qualidade. Atende todos os critérios premium."
}
```

**Lógica de Classificação:**

- **Premium:** TeorFe ≥ 67%, Umidade ≤ 8%, SiO2 ≤ 2%
- **Padrão:** TeorFe ≥ 63%, Umidade ≤ 10%, SiO2 ≤ 4%
- **Baixa:** Demais casos

---

### 2. Preço por Tonelada

**Endpoint:** `GET /api/lotes/{id}/preco`

**Exemplo:** `GET http://localhost:5000/api/lotes/1/preco`

**Resposta:**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "classificacao": "Premium",
  "toneladas": 15000,
  "precoPorTonelada": 850.0,
  "valorTotal": 12750000.0,
  "moeda": "BRL"
}
```

**Tabela de Preços:**

- **Premium:** R$ 850/tonelada
- **Padrão:** R$ 650/tonelada
- **Baixa:** R$ 450/tonelada

---

### 3. Penalidade por Umidade

**Endpoint:** `GET /api/lotes/{id}/penalidade-umidade`

**Exemplo:** `GET http://localhost:5000/api/lotes/1/penalidade-umidade`

**Resposta (com penalidade):**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "umidadeAtual": 12.5,
  "umidadeMaximaPermitida": 10.0,
  "excessoUmidade": 2.5,
  "toneladas": 15000,
  "valorPenalidadePorTonelada": 37.5,
  "valorTotalPenalidade": 562500.0,
  "temPenalidade": true,
  "observacao": "Umidade acima do limite. Penalidade aplicada: 2.50% x R$ 15.00/t/% = R$ 37.50/t"
}
```

**Resposta (sem penalidade):**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "umidadeAtual": 8.0,
  "umidadeMaximaPermitida": 10.0,
  "excessoUmidade": 0.0,
  "toneladas": 15000,
  "valorPenalidadePorTonelada": 0.0,
  "valorTotalPenalidade": 0.0,
  "temPenalidade": false,
  "observacao": "Umidade dentro do limite permitido. Sem penalidades."
}
```

**Lógica:**

- Umidade máxima permitida: 10%
- Penalidade: R$ 15/tonelada para cada 1% de excesso

---

## 📤 Métodos POST (sem body JSON)

### 4. Adicionar Histórico de Movimentação

**Endpoint:** `POST /api/lotes/{id}/movimentacao?localDestino={destino}`

**Exemplo:** `POST http://localhost:5000/api/lotes/1/movimentacao?localDestino=Porto de Tubarão`

**Resposta:**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "dataMovimentacao": "2026-02-12T14:35:22",
  "localOrigem": "Mina Carajás",
  "localDestino": "Porto de Tubarão",
  "statusAnterior": "EmEstoque",
  "statusAtual": "EmEstoque",
  "mensagem": "Lote movimentado de 'Mina Carajás' para 'Porto de Tubarão'"
}
```

**Observação:** O parâmetro `localDestino` é obrigatório e deve ser enviado como query parameter.

---

### 5. Avançar Status do Lote

**Endpoint:** `POST /api/lotes/{id}/avancar-status`

**Exemplo:** `POST http://localhost:5000/api/lotes/1/avancar-status`

**Resposta (EmEstoque → EmTransporte):**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "dataMovimentacao": "2026-02-12T14:40:15",
  "localOrigem": "Mina Carajás",
  "localDestino": "Em trânsito - Via Férrea",
  "statusAnterior": "EmEstoque",
  "statusAtual": "EmTransporte",
  "mensagem": "Lote avançou de 'EmEstoque' para 'EmTransporte'"
}
```

**Resposta (EmTransporte → Embarcado):**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "dataMovimentacao": "2026-02-12T14:45:30",
  "localOrigem": "Em trânsito - Via Férrea",
  "localDestino": "Porto - Navio atracado",
  "statusAnterior": "EmTransporte",
  "statusAtual": "Embarcado",
  "mensagem": "Lote avançou de 'EmTransporte' para 'Embarcado'"
}
```

**Resposta (Já Embarcado):**

```json
{
  "loteId": 1,
  "codigoLote": "MNA-2026-000123",
  "dataMovimentacao": "2026-02-12T14:50:00",
  "localOrigem": "Porto - Navio atracado",
  "localDestino": "Porto - Navio atracado",
  "statusAnterior": "Embarcado",
  "statusAtual": "Embarcado",
  "mensagem": "Lote já está no status final 'Embarcado'. Não é possível avançar."
}
```

**Fluxo de Status:**

1. **EmEstoque** (0) → EmTransporte
2. **EmTransporte** (1) → Embarcado
3. **Embarcado** (2) → Status final

---

## 🧪 Como Testar no Insomnia

### Passo 1: Criar um lote de teste (se necessário)

```
POST http://localhost:5000/api/LotesMinerio
Content-Type: application/json

{
  "codigoLote": "TEST-2026-001",
  "minaOrigem": "Carajás",
  "teorFe": 68.5,
  "umidade": 7.2,
  "siO2": 1.8,
  "toneladas": 15000,
  "status": 0,
  "localizacaoAtual": "Mina Carajás"
}
```

### Passo 2: Testar os endpoints GET

1. Classificação: `GET http://localhost:5000/api/lotes/1/classificacao`
2. Preço: `GET http://localhost:5000/api/lotes/1/preco`
3. Penalidade: `GET http://localhost:5000/api/lotes/1/penalidade-umidade`

### Passo 3: Testar os endpoints POST

1. Movimentação: `POST http://localhost:5000/api/lotes/1/movimentacao?localDestino=Porto%20de%20Tubarão`
2. Avançar Status: `POST http://localhost:5000/api/lotes/1/avancar-status`
3. Avançar Status novamente: `POST http://localhost:5000/api/lotes/1/avancar-status`

---

## 📊 Resumo dos Endpoints

| Método | Endpoint                                        | Descrição                                |
| ------ | ----------------------------------------------- | ---------------------------------------- |
| GET    | `/api/lotes/{id}/classificacao`                 | Classifica qualidade do lote             |
| GET    | `/api/lotes/{id}/preco`                         | Calcula preço e valor total              |
| GET    | `/api/lotes/{id}/penalidade-umidade`            | Calcula penalidade por umidade           |
| POST   | `/api/lotes/{id}/movimentacao?localDestino=...` | Registra movimentação do lote            |
| POST   | `/api/lotes/{id}/avancar-status`                | Avança status do lote no fluxo logístico |

---

## ✅ Implementação Completa

Todos os 5 métodos foram implementados:

1. ✅ Classificação de Qualidade (`LoteService.ClassificarQualidade`)
2. ✅ Preço por Tonelada (`LoteService.CalcularPreco`)
3. ✅ Histórico de Movimentação (`LoteService.AdicionarHistorico`)
4. ✅ Avançar Status do Lote (`LoteService.AvancarStatus`)
5. ✅ Penalidade por Umidade (`LoteService.CalcularPenalidadeUmidade`)

A classe `LoteService` está localizada em `Services/LoteService.cs` e foi registrada no `Program.cs` para injeção de dependência.
