namespace MediCitas.Models
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}