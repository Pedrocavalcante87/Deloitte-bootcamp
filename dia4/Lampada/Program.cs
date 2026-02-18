using System;
using System.Diagnostics;

class program
{
    static void Main(string[] args)
    {
        Lampada lampada = new Lampada();
        bool executando = true;

        while (executando)
        {
            Console.WriteLine("Sistema de controle de Lampada");
            Console.WriteLine("1. Ligar a lampada");
            Console.WriteLine("2. Desligar a lampada");
            Console.WriteLine("3. Verificar status da lampada");
            Console.WriteLine("0. Sair");

            try
            {
                int opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        lampada.Ligar();
                        break;

                    case 2:
                        lampada.Desligar();
                        break;
                    case 3:
                        lampada.Status();
                        break;

                    case 0:
                        executando = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                        break;
                }
            }

            catch (FormatException)
            {
                Console.WriteLine("Entrada inválida. Por favor, insira um número.");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
