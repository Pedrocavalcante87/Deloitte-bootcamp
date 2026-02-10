using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("  DIA 3 - CASE 04 - SISTEMA DE MINA");
        Console.WriteLine("========================================\n");

        // Criar sistema de autenticação
        SistemaAutenticacao auth = new SistemaAutenticacao();

        // Criar Minas
        Mina mina1 = new Mina("M001", "Mina Itabira", 1000);
        Mina mina2 = new Mina("M002", "Mina Carajás", 1500);

        Console.WriteLine($"Mina criada: {mina1.Nome}");
        Console.WriteLine($"Mina criada: {mina2.Nome}\n");

        Producao prod1 = new Producao(1, new DateTime(2026, 1, 15), 800);
        Producao prod2 = new Producao(2, new DateTime(2026, 1, 25), 950);
        Producao prod3 = new Producao(3, new DateTime(2026, 2, 5), 1200);

        mina1.AdicionarProducao(prod1);
        mina1.AdicionarProducao(prod2);
        mina2.AdicionarProducao(prod3);

        Estoque est1 = new Estoque(1, 500, "Armazém A");
        Estoque est2 = new Estoque(2, 300, "Galpão B");
        Estoque est3 = new Estoque(3, 950, "Terminal Portuário");
        Estoque est4 = new Estoque(4, 700, "Pátio Principal");
        Estoque est5 = new Estoque(5, 500, "Estação de Carregamento");

        prod1.AdicionarEstoque(est1);
        prod1.AdicionarEstoque(est2);
        prod2.AdicionarEstoque(est3);
        prod3.AdicionarEstoque(est4);
        prod3.AdicionarEstoque(est5);

        Console.WriteLine("Sistema de mineração configurado com sucesso!\n");

        // Sistema de Login
        Console.WriteLine("========================================");
        Console.WriteLine("       AUTENTICAÇÃO NECESSÁRIA");
        Console.WriteLine("========================================");

        auth.ExibirCredenciaisDisponiveis();

        Console.WriteLine("\nTentando acessar dados sem autenticação...");
        mina1.ExibirResumoBasico();
        mina1.ExibirResumo();

        bool autenticado = false;
        int tentativas = 0;
        int maxTentativas = 3;

        while (!autenticado && tentativas < maxTentativas)
        {
            Console.WriteLine($"\n\n--- Tentativa {tentativas + 1}/{maxTentativas} ---");
            Console.Write("Digite o usuário: ");
            string usuario = Console.ReadLine();

            Console.Write("Digite a senha: ");
            string senha = LerSenhaOculta();

            autenticado = auth.Autenticar(usuario, senha);

            if (autenticado)
            {
                Console.WriteLine("\nSUCESSO: Autenticação realizada com sucesso!");
                Console.WriteLine($"Bem-vindo, {auth.UsuarioAtual}!");
            }
            else
            {
                tentativas++;
                if (tentativas < maxTentativas)
                    Console.WriteLine("\nERRO: Credenciais inválidas! Tente novamente.");
                else
                    Console.WriteLine("\nERRO: Número máximo de tentativas excedido!");
            }
        }

        if (!autenticado)
        {
            Console.WriteLine("\nERRO: Acesso negado ao sistema!");
            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
            return;
        }

        // Liberar acesso às minas
        Console.WriteLine("\n\nLiberando acesso às minas...");
        mina1.LiberarAcesso(auth);
        mina2.LiberarAcesso(auth);

        // Exibir Relatórios (agora com acesso)
        Console.WriteLine("\n\n========================================");
        Console.WriteLine("           RELATÓRIOS");
        Console.WriteLine("========================================");

        mina1.ExibirResumo();
        foreach (var producao in mina1.Producoes)
        {
            producao.ExibirDetalhes();
        }

        mina2.ExibirResumo();
        foreach (var producao in mina2.Producoes)
        {
            producao.ExibirDetalhes();
        }

        Console.WriteLine("\n========================================");
        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    static string LerSenhaOculta()
    {
        string senha = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                senha += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && senha.Length > 0)
            {
                senha = senha.Substring(0, senha.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return senha;
    }
}
