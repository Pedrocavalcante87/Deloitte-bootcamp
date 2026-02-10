using System;
using System.Collections.Generic;

public class SistemaAutenticacao
{
    private Dictionary<string, Credencial> _credenciais;
    private string _usuarioAutenticado;

    public bool EstaAutenticado => !string.IsNullOrEmpty(_usuarioAutenticado);
    public string UsuarioAtual => _usuarioAutenticado;

    public SistemaAutenticacao()
    {
        _credenciais = new Dictionary<string, Credencial>();
        InicializarCredenciais();
    }

    private void InicializarCredenciais()
    {
        // Credenciais pré-cadastradas
        _credenciais.Add("admin", new Credencial("admin", "admin123"));
        _credenciais.Add("operador", new Credencial("operador", "op123"));
        _credenciais.Add("gestor", new Credencial("gestor", "gestor123"));
    }

    public bool Autenticar(string usuario, string senha)
    {
        if (_credenciais.ContainsKey(usuario))
        {
            if (_credenciais[usuario].Validar(usuario, senha))
            {
                _usuarioAutenticado = usuario;
                return true;
            }
        }
        return false;
    }

    public void Deslogar()
    {
        _usuarioAutenticado = null;
    }

    public void ExibirCredenciaisDisponiveis()
    {
        Console.WriteLine("\nCredenciais disponíveis para teste:");
        Console.WriteLine("   - Usuário: admin     | Senha: admin123");
        Console.WriteLine("   - Usuário: operador  | Senha: op123");
        Console.WriteLine("   - Usuário: gestor    | Senha: gestor123");
    }
}
