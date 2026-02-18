using System;

public class Lampada
{
    private bool isligada;

    public Lampada()
    {
        isligada = false;
    }

    public void Ligar()
    {
        isligada = true;
        Console.WriteLine("A lampada está ligada.");
        return;
    }

    public void Desligar()
    {
        if (!isligada)
        {
            Console.WriteLine("A lampada já está desligada. ");
            return;
        }

        isligada = false;
        Console.WriteLine("A lampada está desligada.");
    }
    public void Status()
    {
        Console.WriteLine(isligada ? "A lampada está Ligada." : "A lampada está Desligada.");
    }
}



