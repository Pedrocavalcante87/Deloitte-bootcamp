using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCheckin
{
    public class GerenciadorCheckin
    {
        // ArrayList para armazenar visitantes (requisito do exercício)
        private ArrayList visitantes = new ArrayList();

        public void CadastrarVisitante(string nome, string documento, bool ePrimeiraVez)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome não pode ser vazio!");
            }

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentException("Documento não pode ser vazio!");
            }

            Visitante visitante = new Visitante(nome, documento, ePrimeiraVez);
            visitantes.Add(visitante);

            Console.WriteLine($"\n✓ Visitante {visitante.Nome} cadastrado com sucesso! (ID: {visitante.Id})");
        }

        public void ListarVisitantes()
        {
            if (visitantes.Count == 0)
            {
                Console.WriteLine("Nenhum visitante cadastrado.");
                return;
            }

            // Iterar sobre ArrayList
            foreach (Visitante v in visitantes)
            {
                Console.WriteLine(v.ToString());
            }

            Console.WriteLine($"\nTotal: {visitantes.Count} visitante(s)");
        }

        public void BuscarVisitantePorNome(string termoBusca)
        {
            if (string.IsNullOrWhiteSpace(termoBusca))
            {
                throw new ArgumentException("Digite um nome para buscar!");
            }

            // Converter ArrayList para List<Visitante> para usar LINQ
            List<Visitante> lista = visitantes.Cast<Visitante>().ToList();
            var encontrados = lista.Where(v => v.Nome.ToLower().Contains(termoBusca.ToLower())).ToList();

            if (encontrados.Count == 0)
            {
                Console.WriteLine($"\nNenhum visitante encontrado com o nome '{termoBusca}'.");
                return;
            }

            Console.WriteLine($"\n{encontrados.Count} visitante(s) encontrado(s):\n");
            foreach (var v in encontrados)
            {
                Console.WriteLine(v.ToString());
            }
        }

        public void RegistrarSaida(int id)
        {
            // Buscar visitante no ArrayList
            Visitante visitante = null;
            foreach (Visitante v in visitantes)
            {
                if (v.Id == id)
                {
                    visitante = v;
                    break;
                }
            }

            if (visitante == null)
            {
                throw new ArgumentException($"Visitante com ID {id} não encontrado!");
            }

            if (visitante.HorarioSaida.HasValue)
            {
                Console.WriteLine($"\n⚠ Visitante {visitante.Nome} já registrou saída às {visitante.HorarioSaida:HH:mm:ss}.");
                return;
            }

            visitante.RegistrarSaida();
            Console.WriteLine($"\n✓ Saída registrada para {visitante.Nome} às {visitante.HorarioSaida:HH:mm:ss}");
        }

        public void FiltrarPrimeiraVisita()
        {
            // Converter ArrayList para List<Visitante> e filtrar
            List<Visitante> lista = visitantes.Cast<Visitante>().ToList();
            var primeiraVisita = lista.Where(v => v.EPrimeiraVez).ToList();

            if (primeiraVisita.Count == 0)
            {
                Console.WriteLine("Nenhum visitante de primeira visita cadastrado.");
                return;
            }

            foreach (var v in primeiraVisita)
            {
                Console.WriteLine(v.ToString());
            }

            Console.WriteLine($"\nTotal: {primeiraVisita.Count} visitante(s) de primeira vez");
        }

        public void ListarVisitantesOrdenadosPorId()
        {
            if (visitantes.Count == 0)
            {
                Console.WriteLine("Nenhum visitante cadastrado.");
                return;
            }

            // Converter ArrayList para List<Visitante> e ordenar por ID
            List<Visitante> lista = visitantes.Cast<Visitante>().ToList();
            var ordenados = lista.OrderBy(v => v.Id).ToList();

            foreach (var v in ordenados)
            {
                Console.WriteLine(v.ToString());
            }

            Console.WriteLine($"\nTotal: {ordenados.Count} visitante(s)");
        }
    }
}
