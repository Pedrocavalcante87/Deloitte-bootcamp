using System;

namespace SistemaCheckin
{
    class Program
    {
        static GerenciadorCheckin gerenciador = new GerenciadorCheckin();

        static void Main(string[] args)
        {
            bool continuar = true;

            while (continuar)
            {
                try
                {
                    MostrarMenu();
                    Console.Write("\nEscolha uma opção: ");
                    string opcao = Console.ReadLine();

                    // Estrutura de controle switch
                    switch (opcao)
                    {
                        case "1":
                            CadastrarVisitante();
                            break;
                        case "2":
                            ListarVisitantes();
                            break;
                        case "3":
                            BuscarVisitantePorNome();
                            break;
                        case "4":
                            RegistrarSaida();
                            break;
                        case "5":
                            FiltrarPrimeiraVisita();
                            break;
                        case "6":
                            ListarVisitantesOrdenadosPorId();
                            break;
                        case "0":
                            Console.WriteLine("\nEncerrando o sistema...");
                            continuar = false;
                            break;
                        default:
                            Console.WriteLine("\nOpção inválida! Tente novamente.");
                            break;
                    }

                    if (continuar)
                    {
                        Console.WriteLine("\nPressione qualquer tecla para continuar...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    Console.WriteLine($"\n❌ Erro: {ex.Message}");
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("   SISTEMA DE CONTROLE DE CHECK-IN");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("1 - Cadastrar visitante");
            Console.WriteLine("2 - Listar visitantes cadastrados");
            Console.WriteLine("3 - Buscar visitante pelo nome");
            Console.WriteLine("4 - Registrar saída de visitante");
            Console.WriteLine("5 - Filtrar apenas primeira visita");
            Console.WriteLine("6 - Listar visitantes ordenados por ID");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("════════════════════════════════════════");
        }

        static void CadastrarVisitante()
        {
            Console.Clear();
            Console.WriteLine("═══ CADASTRAR VISITANTE ═══\n");

            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Documento (CPF/RG): ");
            string documento = Console.ReadLine();

            Console.Write("É a primeira vez no coworking? (S/N): ");
            string respostaPrimeiraVez = Console.ReadLine()?.ToUpper();
            bool ePrimeiraVez = respostaPrimeiraVez == "S";

            gerenciador.CadastrarVisitante(nome, documento, ePrimeiraVez);
        }

        static void ListarVisitantes()
        {
            Console.Clear();
            Console.WriteLine("═══ VISITANTES CADASTRADOS ═══\n");
            gerenciador.ListarVisitantes();
        }

        static void BuscarVisitantePorNome()
        {
            Console.Clear();
            Console.WriteLine("═══ BUSCAR VISITANTE ═══\n");

            Console.Write("Digite o nome (ou parte dele): ");
            string termoBusca = Console.ReadLine();

            gerenciador.BuscarVisitantePorNome(termoBusca);
        }

        static void RegistrarSaida()
        {
            Console.Clear();
            Console.WriteLine("═══ REGISTRAR SAÍDA ═══\n");

            Console.Write("Digite o ID do visitante: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                throw new ArgumentException("ID inválido! Digite apenas números.");
            }

            gerenciador.RegistrarSaida(id);
        }

        static void FiltrarPrimeiraVisita()
        {
            Console.Clear();
            Console.WriteLine("═══ VISITANTES - PRIMEIRA VISITA ═══\n");
            gerenciador.FiltrarPrimeiraVisita();
        }

        static void ListarVisitantesOrdenadosPorId()
        {
            Console.Clear();
            Console.WriteLine("═══ VISITANTES ORDENADOS POR ID ═══\n");
            gerenciador.ListarVisitantesOrdenadosPorId();
        }
    }
}
