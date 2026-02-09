using System;

namespace SistemaCheckin
{
    // Classe Visitante com properties (get/set)
    public class Visitante
    {
        private static int _proximoId = 1;

        public int Id { get; private set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public DateTime HorarioChegada { get; set; }
        public bool EPrimeiraVez { get; set; }
        public DateTime? HorarioSaida { get; set; }

        public Visitante(string nome, string documento, bool ePrimeiraVez)
        {
            Id = _proximoId++;
            Nome = nome;
            Documento = documento;
            HorarioChegada = DateTime.Now;
            EPrimeiraVez = ePrimeiraVez;
            HorarioSaida = null;
        }

        public void RegistrarSaida()
        {
            HorarioSaida = DateTime.Now;
        }

        public override string ToString()
        {
            string status = HorarioSaida.HasValue ? $"Saiu às {HorarioSaida:HH:mm:ss}" : "No local";
            string primeiraVez = EPrimeiraVez ? "Sim" : "Não";
            return $"[ID: {Id}] {Nome} | Doc: {Documento} | Chegada: {HorarioChegada:HH:mm:ss} | Primeira vez: {primeiraVez} | Status: {status}";
        }
    }
}
