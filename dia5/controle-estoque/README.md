# Controle de Estoque de Minério

Sistema simples para gerenciar o estoque de minério em uma operação de mineração.

## O que faz

Este programa permite controlar a quantidade de minério armazenada em um depósito. Você informa o tipo de minério (ferro, ouro, cobre, etc.) e o local onde ele está guardado. A partir daí, pode registrar quando entra mais minério no estoque ou quando sai para processamento ou venda.

O sistema garante que você nunca retire mais minério do que tem disponível. Também impede valores negativos ou zero nas movimentações, porque não faz sentido adicionar ou retirar nada sem quantidade.

## Como usar

Execute o programa com `dotnet run` dentro da pasta do projeto.

Primeiro você vai informar qual tipo de minério quer controlar e onde ele está armazenado. Depois disso, um menu aparece com três opções principais: registrar entrada de material, registrar saída e consultar quanto tem no momento.

Quando você consulta o estoque, além de ver a quantidade em quilos, o sistema avisa se o estoque está zerado ou se ainda tem material disponível.

## Regras

O programa só aceita valores maiores que zero para entrada ou saída. Se você tentar retirar mais do que tem, ele avisa quanto está disponível e cancela a operação. Isso evita que o estoque fique negativo, o que seria impossível na vida real.

Se você digitar algo que não seja um número válido, o sistema pede para tentar novamente sem travar o programa.

## Estrutura

O código está dividido em dois arquivos. O EstoqueMinerio.cs contém toda a lógica de controle do estoque com os métodos para entrada, saída e consulta. O Program.cs cuida da interface com o usuário, mostrando o menu e coletando as informações digitadas.
