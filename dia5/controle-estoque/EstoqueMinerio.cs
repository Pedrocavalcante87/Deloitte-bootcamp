using System;

namespace ControleEstoque
{
    public class EstoqueMinerio
    {
        private decimal quantidade;

        public string Tipo { get; private set; }
        public string Local { get; private set; }

        public EstoqueMinerio(string tipo, string local)
        {
            Tipo = tipo;
            Local = local;
            quantidade = 0;
        }

        public void RegistrarEntrada(decimal valor)
        {
            // Entrada deve ser maior que zero
            if (valor <= 0)
            {
                Console.WriteLine("\nErro: A quantidade deve ser maior que zero.");
                return;
            }

            quantidade += valor;
            Console.WriteLine("\n========================================");
            Console.WriteLine("ENTRADA REGISTRADA COM SUCESSO");
            Console.WriteLine("========================================");
            Console.WriteLine($"Minério: {Tipo}");
            Console.WriteLine($"Local: {Local}");
            Console.WriteLine($"Quantidade adicionada: +{valor} kg");
            Console.WriteLine($"Estoque anterior: {quantidade - valor} kg");
            Console.WriteLine($"Estoque atual: {quantidade} kg");
            Console.WriteLine("========================================");
        }

        public void RegistrarSaida(decimal valor)
        {
            // Saída deve ser maior que zero
            if (valor <= 0)
            {
                Console.WriteLine("\nErro: A quantidade deve ser maior que zero.");
                return;
            }

            // Não permitir saída maior que o estoque atual
            if (valor > quantidade)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("ERRO - ESTOQUE INSUFICIENTE");
                Console.WriteLine("========================================");
                Console.WriteLine($"Minério: {Tipo}");
                Console.WriteLine($"Local: {Local}");
                Console.WriteLine($"Disponível: {quantidade} kg");
                Console.WriteLine($"Tentou retirar: {valor} kg");
                Console.WriteLine($"Faltam: {valor - quantidade} kg");
                Console.WriteLine("========================================");
                return;
            }

            decimal quantidadeAnterior = quantidade;
            quantidade -= valor;
            Console.WriteLine("\n========================================");
            Console.WriteLine("SAIDA REGISTRADA COM SUCESSO");
            Console.WriteLine("========================================");
            Console.WriteLine($"Minério: {Tipo}");
            Console.WriteLine($"Local: {Local}");
            Console.WriteLine($"Quantidade retirada: -{valor} kg");
            Console.WriteLine($"Estoque anterior: {quantidadeAnterior} kg");
            Console.WriteLine($"Estoque atual: {quantidade} kg");

            if (quantidade == 0)
            {
                Console.WriteLine("\nATENCAO: Estoque está ZERADO!");
            }
            else if (quantidade < 100)
            {
                Console.WriteLine("\nATENCAO: Estoque está baixo!");
            }

            Console.WriteLine("========================================");
        }

        public decimal ConsultarQuantidade()
        {
            return quantidade;
        }

        public bool EstaZerado()
        {
            return quantidade == 0;
        }
    }
}
