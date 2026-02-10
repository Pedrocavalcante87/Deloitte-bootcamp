using System;
using System.Collections.Generic;
using System.Linq;

public class Producao
{
    private int _id;
    private string _minaCodigo;
    private DateTime _data;
    private decimal _volume;
    private List<Estoque> _estoques;

    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    public string MinaCodigo
    {
        get { return _minaCodigo; }
        private set { _minaCodigo = value; }
    }

    public DateTime Data
    {
        get { return _data; }
        set
        {
            if (value > DateTime.Now)
                throw new ArgumentException("Data da produção não pode ser futura.");
            _data = value;
        }
    }

    public decimal Volume
    {
        get { return _volume; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Volume deve ser maior que zero.");
            _volume = value;
        }
    }

    public IReadOnlyList<Estoque> Estoques
    {
        get { return _estoques.AsReadOnly(); }
    }

    public Producao(int id, DateTime data, decimal volume)
    {
        Id = id;
        Data = data;
        Volume = volume;
        _estoques = new List<Estoque>();
    }

    internal void DefinirMinaCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código da mina não pode ser vazio.");
        _minaCodigo = codigo;
    }

    public void AdicionarEstoque(Estoque estoque)
    {
        if (estoque == null)
            throw new ArgumentNullException(nameof(estoque));

        estoque.DefinirProducaoId(this.Id);
        _estoques.Add(estoque);
    }

    public decimal ObterQuantidadeTotalEstoque()
    {
        return _estoques.Sum(e => e.Quantidade);
    }

    public void ExibirDetalhes()
    {
        Console.WriteLine($"\n  > Produção #{Id} - Data: {Data:dd/MM/yyyy}");
        Console.WriteLine($"    Volume: {Volume} toneladas");
        Console.WriteLine($"    Estoques: {_estoques.Count}");

        foreach (var estoque in _estoques)
        {
            Console.WriteLine($"      - Estoque #{estoque.Id}: {estoque.Quantidade} ton em {estoque.Local}");
        }
    }
}
