using System;

public class Credencial
{
    private string _usuario;
    private string _senha;

    public string Usuario
    {
        get { return _usuario; }
        private set { _usuario = value; }
    }

    public Credencial(string usuario, string senha)
    {
        _usuario = usuario;
        _senha = senha;
    }

    public bool Validar(string usuario, string senha)
    {
        return _usuario == usuario && _senha == senha;
    }
}
