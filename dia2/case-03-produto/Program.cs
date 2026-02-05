// CASE 02 - Sistema de Cadastro de Produtos com Validação

// ===== CLASSE PRODUTO =====
// Esta classe representa um produto do estoque
class Produto
{
    // Properties com get público e set privado (encapsulamento)
    public string Nome { get; private set; }
    public double Preco { get; private set; }
    public int Quantidade { get; private set; }

    // Construtor - cria um produto com validação
    public Produto(string nome, double preco, int quantidade)
    {
        // Validação 1: Nome não pode ser vazio
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("ERRO: O nome do produto não pode ser vazio!");
        }

        // Validação 2: Preço deve ser maior que zero
        if (preco <= 0)
        {
            throw new ArgumentException("ERRO: O preço deve ser maior que zero!");
        }

        // Validação 3: Quantidade não pode ser negativa
        if (quantidade < 0)
        {
            throw new ArgumentException("ERRO: A quantidade não pode ser negativa!");
        }

        // Se passou em todas as validações, atribui os valores
        Nome = nome;
        Preco = preco;
        Quantidade = quantidade;
    }

    // Método para exibir os dados do produto
    public void ExibirDados()
    {
        Console.WriteLine($"Nome: {Nome} | Preço: R$ {Preco:F2} | Quantidade: {Quantidade}");
    }
}

// ===== CLASSE PRINCIPAL =====
class Program
{
    static void Main(string[] args)
    {
        // Lista para armazenar os produtos cadastrados
        List<Produto> estoque = new List<Produto>();

        Console.WriteLine("==================================");
        Console.WriteLine("   SISTEMA DE CADASTRO DE PRODUTOS");
        Console.WriteLine("==================================\n");

        // Loop principal do programa
        bool continuar = true;
        while (continuar)
        {
            // Menu de opções
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1 - Cadastrar novo produto");
            Console.WriteLine("2 - Listar produtos cadastrados");
            Console.WriteLine("3 - Sair");
            Console.Write("Opção: ");

            string opcao = Console.ReadLine();

            // Estrutura condicional para processar a opção escolhida
            if (opcao == "1")
            {
                CadastrarProduto(estoque);
            }
            else if (opcao == "2")
            {
                ListarProdutos(estoque);
            }
            else if (opcao == "3")
            {
                continuar = false;
                Console.WriteLine("\nEncerrando o sistema... Até logo!");
            }
            else
            {
                Console.WriteLine("\n❌ Opção inválida! Tente novamente.");
            }
        }
    }

    // ===== MÉTODO PARA CADASTRAR PRODUTO =====
    static void CadastrarProduto(List<Produto> estoque)
    {
        Console.WriteLine("\n--- CADASTRO DE NOVO PRODUTO ---");

        // Coleta os dados do produto
        Console.Write("Nome do produto: ");
        string nome = Console.ReadLine();

        Console.Write("Preço: R$ ");
        string precoTexto = Console.ReadLine();

        Console.Write("Quantidade em estoque: ");
        string quantidadeTexto = Console.ReadLine();

        // Validação SEM try/catch usando estruturas condicionais

        // Valida se o preço é um número válido
        double preco;
        bool precoValido = double.TryParse(precoTexto, out preco);

        if (!precoValido)
        {
            Console.WriteLine("\n❌ ERRO: Preço inválido! Digite um número.");
            return;
        }

        // Valida se a quantidade é um número inteiro válido
        int quantidade;
        bool quantidadeValida = int.TryParse(quantidadeTexto, out quantidade);

        if (!quantidadeValida)
        {
            Console.WriteLine("\n❌ ERRO: Quantidade inválida! Digite um número inteiro.");
            return;
        }

        // Validações de regras de negócio usando if/else

        // Validação 1: Nome não pode ser vazio
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("\n❌ ERRO: O nome do produto não pode ser vazio!");
            return;
        }

        // Validação 2: Preço deve ser maior que zero
        if (preco <= 0)
        {
            Console.WriteLine("\n❌ ERRO: O preço deve ser maior que zero!");
            return;
        }

        // Validação 3: Quantidade não pode ser negativa
        if (quantidade < 0)
        {
            Console.WriteLine("\n❌ ERRO: A quantidade não pode ser negativa!");
            return;
        }

        // Se todas as validações passaram, cria o produto
        Produto novoProduto = new Produto(nome, preco, quantidade);
        estoque.Add(novoProduto);

        Console.WriteLine("\n✅ Produto cadastrado com sucesso!");
        novoProduto.ExibirDados();
    }

    // ===== MÉTODO PARA LISTAR PRODUTOS =====
    static void ListarProdutos(List<Produto> estoque)
    {
        Console.WriteLine("\n--- PRODUTOS CADASTRADOS ---");

        // Verifica se há produtos no estoque
        if (estoque.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado ainda.");
            return;
        }

        // Lista todos os produtos
        for (int i = 0; i < estoque.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            estoque[i].ExibirDados();
        }

        Console.WriteLine($"\nTotal de produtos: {estoque.Count}");
    }
}
