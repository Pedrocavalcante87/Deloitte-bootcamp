using System;
using System.Collections.Generic;
using System.Linq;

public class Mina
{
    private string _codigo;
    private string _nome;
    private decimal _capacidade;
    private List<Producao> _producoes;
    private bool _acessoLiberado;

    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Código da mina não pode ser vazio.");
            _codigo = value;
        }
    }

    public string Nome
    {
        get { return _nome; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Nome da mina não pode ser vazio.");
            _nome = value;
        }
    }

    public decimal Capacidade
    {
        get { return _capacidade; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Capacidade deve ser maior que zero.");
            _capacidade = value;
        }
    }

    // Retorna coleção somente leitura
    public IReadOnlyList<Producao> Producoes
    {
        get { return _producoes.AsReadOnly(); }
    }

    public Mina(string codigo, string nome, decimal capacidade)
    {
        Codigo = codigo;
        Nome = nome;
        Capacidade = capacidade;
        _producoes = new List<Producao>();
        _acessoLiberado = false;
    }

    public bool LiberarAcesso(SistemaAutenticacao sistema)
    {
        if (sistema == null || !sistema.EstaAutenticado)
        {
            Console.WriteLine("ERRO: Acesso negado! Autenticação necessária.");
            return false;
        }

        _acessoLiberado = true;
        Console.WriteLine($"Acesso liberado para {sistema.UsuarioAtual}");
        return true;
    }

    public void BloquearAcesso()
    {
        _acessoLiberado = false;
    }

    private bool VerificarAcesso(string operacao)
    {
        if (!_acessoLiberado)
        {
            Console.WriteLine($"BLOQUEADO: Acesso bloqueado para '{operacao}' na mina {Nome}");
            return false;
        }
        return true;
    }

    public void AdicionarProducao(Producao producao)
    {
        if (producao == null)
            throw new ArgumentNullException(nameof(producao));

        if (producao.Volume > _capacidade)
            Console.WriteLine($"AVISO: Volume {producao.Volume} excede capacidade {_capacidade}!");

        producao.DefinirMinaCodigo(this.Codigo);
        _producoes.Add(producao);
    }

    public decimal ObterProducaoTotal()
    {
        return _producoes.Sum(p => p.Volume);
    }

    public void ExibirResumo()
    {
        if (!VerificarAcesso("exibir resumo"))
            return;

        Console.WriteLine($"\n=== MINA: {Nome} ===");
        Console.WriteLine($"Código: {Codigo}");
        Console.WriteLine($"Capacidade: {Capacidade} toneladas");
        Console.WriteLine($"Total de Produções: {_producoes.Count}");
        Console.WriteLine($"Volume Total Produzido: {ObterProducaoTotal()} toneladas");
    }

    public void ExibirResumoBasico()
    {
        Console.WriteLine($"\n=== MINA: {Nome} (Código: {Codigo}) ===");
        Console.WriteLine("Dados detalhados requerem autenticação");
    }
}
