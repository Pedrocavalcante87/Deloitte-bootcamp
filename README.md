# 🎓 Bootcamp Deloitte .NET

Repositório do bootcamp intensivo de desenvolvimento .NET, com foco em aplicações console, APIs REST e boas práticas de desenvolvimento.

---

## 📚 Estrutura do Bootcamp

### **Dia 0** - Fundamentos Git

**Objetivo:** Padronização de commits e organização de repositório

- ✅ Fluxo de branches (`main` e `develop`)
- ✅ Padrão de commits e mensagens
- ✅ Navegação no terminal PowerShell
- 📁 [case-01-commit-padrao](dia0/case-01-commit-padrao)

---

### **Dia 1** - Fundamentos .NET e Validação

**Objetivo:** Introdução ao .NET e validação de entrada de dados

#### Case 1: API Piloto

- ✅ Criação de Web API básica em .NET
- ✅ Exploração do Swagger
- 📁 [api-piloto](dia1/api-piloto)

#### Case 2: Validação de Entrada

- ✅ Tratamento de exceções com `try/catch`
- ✅ Validação de nome e idade
- ✅ Uso de `TryParse` para conversão segura
- 📁 [case-02](dia1/case-02)

---

### **Dia 2** - POO e Estruturas de Dados

**Objetivo:** Programação Orientada a Objetos e coleções

#### Case 3: Sistema de Produtos

- ✅ Classes e encapsulamento (`properties`)
- ✅ Validações de regras de negócio
- ✅ Uso de `List<T>` para coleções
- ✅ Menu interativo em console
- 📁 [case-03-produto](dia2/case-03-produto)

#### Case 3.1: Sistema de Check-in

- ✅ Controle de visitantes com menu
- ✅ Uso de `ArrayList` e `List<T>`
- ✅ Operações CRUD em console
- ✅ Busca e registro de saída
- 📁 [case-3.1-checkin](dia2/case-3.1-checkin)

---

### **Dia 3** - Relacionamentos e Autenticação

**Objetivo:** Modelagem de entidades relacionadas e controle de acesso

#### Case 4: Sistema de Mina

- ✅ Relacionamento entre entidades (Mina → Produção → Estoque)
- ✅ Sistema de autenticação básico
- ✅ Controle de acesso com credenciais
- ✅ Diagrama ER com Mermaid
- 📁 [case-04-mina](dia3/case-04-mina)

---

### **Dia 4** - Orientação a Objetos Avançada

**Objetivo:** Conceitos avançados de POO

#### Case: Sistema de Lâmpada

- ✅ Controle de estado (ligada/desligada)
- ✅ Encapsulamento de comportamento
- ✅ Menu interativo com `switch`
- 📁 [Lampada](dia4/Lampada)

---

### **Dia 5** - Gestão de Estoque

**Objetivo:** Sistema robusto de controle de entrada e saída

#### Case: Controle de Estoque de Minério

- ✅ Gerenciamento de estoque com validações
- ✅ Controle de entrada e saída de material
- ✅ Regras de negócio (não permitir estoque negativo)
- ✅ Separação de responsabilidades (lógica vs interface)
- 📁 [controle-estoque](dia5/controle-estoque)

---

### **Projeto Avançado** - Gestão de Lotes de Minério

**Objetivo:** API REST completa com arquitetura avançada e tecnologias empresariais

#### 🏭 Sistema de Gestão de Lotes de Minério

Projeto intermediário desenvolvido durante o bootcamp para aplicar conceitos avançados de arquitetura, processamento assíncrono e containerização.

**Nota:** _O desafio final do bootcamp está disponível em repositório separado._

**Funcionalidades implementadas:**

**Tecnologias:**

- .NET 10.0 + Entity Framework Core
- PostgreSQL (banco de dados)
- Redis (cache distribuído + filas)
- Docker Compose
- xUnit + Testcontainers (testes)

**Funcionalidades:**

- ✅ CRUD completo de lotes de minério
- ✅ Processamento assíncrono com **Redis Streams**
- ✅ Cache distribuído para otimização
- ✅ Classificação automática de qualidade (Premium/Padrão/Baixa)
- ✅ Controle logístico com rastreamento
- ✅ Cálculos de precificação dinâmica
- ✅ Resiliência com DLQ (Dead Letter Queue)
- ✅ Testes de integração completos

**Arquitetura:**

```
Controllers → Services → Data (EF Core) → PostgreSQL
              ↓
         Redis Streams (Processamento Assíncrono)
         Redis Cache (Otimização)
```

📁 [GestaoLotesMinerio](GestaoLotesMinerio)

---

## 🚀 Como Executar os Projetos

### Aplicações Console

```bash
cd <diretorio-do-case>
dotnet run
```

### APIs REST

```bash
cd <diretorio-da-api>
dotnet run
# Acesse: https://localhost:7xxx/swagger
```

### Gestão de Lotes (com Docker)

```bash
cd GestaoLotesMinerio/MinhaApi
docker-compose up -d
dotnet run
```

---

## 📖 Conceitos Trabalhados

### Fundamentos C#

- Tipos primitivos e conversões
- Estruturas de controle (if/else, switch, loops)
- Tratamento de exceções
- Validação de entrada

### Programação Orientada a Objetos

- Classes e objetos
- Encapsulamento (properties)
- Relacionamentos entre entidades
- Separação de responsabilidades

### Estruturas de Dados

- Arrays e ArrayList
- List<T> (genéricos)
- Dicionários e coleções

### Desenvolvimento Web

- ASP.NET Core Web API
- Controllers e DTOs
- Entity Framework Core
- Swagger/OpenAPI

### Arquitetura e Boas Práticas

- Camadas (Controllers, Services, Data)
- Injeção de Dependência
- Processamento assíncrono
- Cache distribuído
- Padrão Repository

### DevOps e Infraestrutura

- Docker e Docker Compose
- Bancos de dados (PostgreSQL)
- Cache e Filas (Redis)
- Testes automatizados (xUnit, Testcontainers)

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia            | Uso                             |
| --------------------- | ------------------------------- |
| C# / .NET             | Linguagem e framework principal |
| ASP.NET Core          | APIs REST                       |
| Entity Framework Core | ORM para banco de dados         |
| PostgreSQL            | Banco de dados relacional       |
| Redis                 | Cache e filas (Streams)         |
| Docker                | Containerização                 |
| xUnit                 | Framework de testes             |
| Git                   | Controle de versão              |

---

## 📝 Padrão de Commits

O bootcamp segue um padrão consistente de commits:

```
feat: adiciona nova funcionalidade
fix: corrige bug
docs: atualiza documentação
refactor: refatora código
test: adiciona testes
```

**Branch principal de desenvolvimento:** `develop`
**Branch estável:** `main`

---

## 👨‍💻 Autor

Desenvolvido durante o **Bootcamp Deloitte .NET**

---

## 📄 Licença

Este projeto é parte de um programa educacional da Deloitte.
