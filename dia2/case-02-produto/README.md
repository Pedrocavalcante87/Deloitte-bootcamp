# Dia 02 — Case 2: Sistema de Cadastro de Produtos com Validação

## Sobre a atividade

Uma pequena loja estava tendo problemas com produtos sendo cadastrados com informações incorretas (nome vazio, preço zero, quantidade negativa). A solução foi criar um sistema em C# que valida todos os dados antes de adicionar o produto ao estoque.

O programa usa um menu interativo onde você pode cadastrar produtos, listar o estoque e garantir que nenhum dado inválido seja registrado.

## O que foi implementado

- **Classe Produto** com properties encapsuladas (`get` público e `set` privado)
- **Validações de regras de negócio** usando apenas estruturas condicionais (if/else):
  - Nome não pode ser vazio
  - Preço deve ser maior que zero
  - Quantidade não pode ser negativa
- **List<Produto>** para armazenar o estoque dinamicamente
- **Menu interativo** com 3 opções: cadastrar, listar e sair
- **Validação de entrada** sem usar try/catch, apenas if/else e TryParse

## Conceitos aplicados

Estrutura básica do C# (class, Main)
Tipos primitivos (string, int, double)
Estrutura condicional (if/else)
Properties com encapsulamento
List<T> para coleções
Validação sem try/catch

## Como executar

```bash
cd dia2/case-02-produto
dotnet run
```

Depois é só seguir o menu e testar tanto casos válidos quanto inválidos para ver as validações funcionando!
