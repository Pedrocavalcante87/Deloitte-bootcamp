using System;

public class Estoque
{
    private int _id;
    private int _producaoId;
    private decimal _quantidade;
    private string _local;

    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    public int ProducaoId
    {
        get { return _producaoId; }
        private set { _producaoId = value; }
    }

    public decimal Quantidade
    {
        get { return _quantidade; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Quantidade não pode ser negativa.");
            _quantidade = value;
        }
    }

    public string Local
    {
        get { return _local; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Local do estoque não pode ser vazio.");
            _local = value;
        }
    }

    public Estoque(int id, decimal quantidade, string local)
    {
        Id = id;
        Quantidade = quantidade;
        Local = local;
    }

    internal void DefinirProducaoId(int producaoId)
    {
        if (producaoId <= 0)
            throw new ArgumentException("ID da produção inválido.");
        _producaoId = producaoId;
    }

    public void ExibirInfo()
    {
        Console.WriteLine($"Estoque #{Id} - Local: {Local} - Quantidade: {Quantidade} toneladas");
    }
}
