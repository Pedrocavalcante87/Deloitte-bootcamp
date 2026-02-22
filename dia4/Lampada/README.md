# Dia 04 — Sistema de Controle de Lâmpada

## Sobre a atividade

Este exercício aplica conceitos de **Programação Orientada a Objetos (POO)** em C#, criando um sistema simples de controle de estado de uma lâmpada. O objetivo é praticar encapsulamento, controle de estado e criação de menus interativos.

A lâmpada pode estar ligada ou desligada, e o usuário interage com ela através de um menu em console.

---

## O que foi implementado

### Classe `Lampada`

- **Atributo privado** `isligada` (bool) para controlar o estado
- **Construtor** que inicializa a lâmpada como desligada
- **Método `Ligar()`** - Liga a lâmpada e exibe mensagem
- **Método `Desligar()`** - Desliga a lâmpada com validação (não permite desligar se já estiver desligada)
- **Método `Status()`** - Consulta e exibe o estado atual usando operador ternário

### Program.cs - Interface com usuário

- **Menu interativo** em loop com 4 opções:
  1. Ligar a lâmpada
  2. Desligar a lâmpada
  3. Verificar status
  4. Sair
- **Estrutura `switch`** para controlar as opções do menu
- **Tratamento de exceções**:
  - `FormatException` para entradas não numéricas
  - `Exception` genérica para outros erros

---

## Conceitos aplicados

- **Encapsulamento**: atributo privado acessado apenas por métodos públicos
- **Controle de estado**: a lâmpada mantém seu estado (ligada/desligada)
- **Validação de negócio**: impede desligar uma lâmpada já desligada
- **Operador ternário**: `isligada ? "Ligada" : "Desligada"`
- **Menu com switch**: estrutura de controle para múltiplas opções
- **Tratamento de exceções**: `try/catch` para validar entrada do usuário
- **Loop infinito controlado**: `while(executando)` com flag booleana

---

## Como executar

```bash
cd dia4/Lampada
dotnet run
```

### Exemplo de uso

```
Sistema de controle de Lampada
1. Ligar a lampada
2. Desligar a lampada
3. Verificar status da lampada
0. Sair
> 1
A lampada está ligada.

Sistema de controle de Lampada
1. Ligar a lampada
2. Desligar a lampada
3. Verificar status da lampada
0. Sair
> 3
A lampada está Ligada.

Sistema de controle de Lampada
1. Ligar a lampada
2. Desligar a lampada
3. Verificar status da lampada
0. Sair
> 2
A lampada está desligada.

Sistema de controle de Lampada
1. Ligar a lampada
2. Desligar a lampada
3. Verificar status da lampada
0. Sair
> 2
A lampada já está desligada.
```

---

## Estrutura do projeto

```
Lampada/
├── lampada.cs         # Classe Lampada com lógica de controle
├── Program.cs         # Interface do usuário (menu)
├── Lampada.csproj     # Configuração do projeto
└── README.md          # Este arquivo
```

---

## Validações implementadas

- ✅ Impede desligar uma lâmpada que já está desligada
- ✅ Valida entrada numérica no menu (trata `FormatException`)
- ✅ Exibe mensagem de erro para opções inválidas
- ✅ Permite sair do programa de forma controlada (opção 0)

---

## Aprendizados

Este exercício demonstra como aplicar POO em um cenário simples:

- Separação entre lógica de negócio (`Lampada.cs`) e interface (`Program.cs`)
- Uso adequado de modificadores de acesso (`private`, `public`)
- Controle de estado através de métodos
- Interação com o usuário via menu em console
