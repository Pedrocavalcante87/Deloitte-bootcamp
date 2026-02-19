using System;
using ControleEstoque;

class Program
{
    static EstoqueMinerio estoque;

    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("  CONTROLE DE ESTOQUE DE MINÉRIO");
        Console.WriteLine("========================================\n");

        Console.Write("Digite o tipo de minério (ex: Ferro, Ouro): ");
        string tipo = Console.ReadLine();

        Console.Write("Digite o local de armazenamento (ex: Depósito A): ");
        string local = Console.ReadLine();

        estoque = new EstoqueMinerio(tipo, local);

        Console.WriteLine($"\nEstoque criado com sucesso!");
        Console.WriteLine($"Tipo: {tipo}");
        Console.WriteLine($"Local: {local}\n");

        ExibirMenu();
    }

    static void ExibirMenu()
    {
        while (true)
        {
            MostrarResumoEstoque();
            Console.WriteLine("\n========================================");
            Console.WriteLine("MENU:");
            Console.WriteLine("  1 - Adicionar minério (entrada)");
            Console.WriteLine("  2 - Retirar minério (saída)");
            Console.WriteLine("  3 - Ver detalhes completos");
            Console.WriteLine("  0 - Sair do sistema");
            Console.WriteLine("========================================");
            Console.Write("Digite sua opção: ");

            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    RegistrarEntrada();
                    break;
                case "2":
                    RegistrarSaida();
                    break;
                case "3":
                    ConsultarEstoque();
                    break;
                case "0":
                    Console.WriteLine("\nSistema encerrado. Até logo!");
                    return;
                default:
                    Console.WriteLine("\nOpção inválida! Escolha 1, 2, 3 ou 0.");
                    break;
            }
        }
    }

    static void RegistrarEntrada()
    {
        try
        {
            Console.Write("\nQuantidade a adicionar (kg): ");
            decimal valor = decimal.Parse(Console.ReadLine());
            estoque.RegistrarEntrada(valor);
        }
        catch (FormatException)
        {
            Console.WriteLine("\nErro: Digite apenas números (ex: 100 ou 150.5)");
        }
    }

    static void RegistrarSaida()
    {
        try
        {
            Console.Write("\nQuantidade a retirar (kg): ");
            decimal valor = decimal.Parse(Console.ReadLine());
            estoque.RegistrarSaida(valor);
        }
        catch (FormatException)
        {
            Console.WriteLine("\nErro: Digite apenas números (ex: 50 ou 75.5)");
        }
    }

    static void ConsultarEstoque()
    {
        decimal quantidade = estoque.ConsultarQuantidade();
        string status = estoque.EstaZerado() ? "CRITICO - ESTOQUE ZERADO" : "OK";

        Console.WriteLine("\n========================================");
        Console.WriteLine("DETALHES DO ESTOQUE");
        Console.WriteLine("========================================");
        Console.WriteLine($"Tipo: {estoque.Tipo}");
        Console.WriteLine($"Local: {estoque.Local}");
        Console.WriteLine($"Quantidade: {quantidade} kg");
        Console.WriteLine($"Status: {status}");
        Console.WriteLine("========================================");
    }

    static void MostrarResumoEstoque()
    {
        decimal quantidade = estoque.ConsultarQuantidade();
        string status = estoque.EstaZerado() ? "ZERADO" : "OK";

        Console.WriteLine($"\n{estoque.Tipo} - {estoque.Local}");
        Console.WriteLine($"Estoque atual: {quantidade} kg | Status: {status}");
    }
}
