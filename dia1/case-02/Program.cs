using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== CASE 02 | Validação de Entrada ===");

        string nome = LerNome();
        int idade = LerIdade();

        Console.WriteLine();
        Console.WriteLine("Cadastro realizado com sucesso!");
        Console.WriteLine($"Nome: {nome}");
        Console.WriteLine($"Idade: {idade}");
    }

    private static string LerNome()
    {
        while (true)
        {
            try
            {
                Console.Write("Digite o nome: ");
                string nome = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nome))
                    return nome.Trim();

                Console.WriteLine("Erro: o nome não pode ser vazio.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                Console.WriteLine("Tente novamente.");
            }
        }
    }

    private static int LerIdade()
    {
        while (true)
        {
            try
            {
                Console.Write("Digite a idade: ");
                string idadeTexto = Console.ReadLine();

                if (!int.TryParse(idadeTexto, out int idade))
                {
                    Console.WriteLine("Erro: a idade deve ser um número válido.");
                    continue;
                }

                if (idade <= 0)
                {
                    Console.WriteLine("Erro: a idade deve ser maior que zero.");
                    continue;
                }

                return idade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                Console.WriteLine("Tente novamente.");
            }
        }
    }
}
