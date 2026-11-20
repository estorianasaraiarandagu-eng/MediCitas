namespace MediCitas.Models
{
    public class ConsultaMedica
    {
        public int IdConsulta { get; set; }
        public int IdCita { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string Receta { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        
        // Propiedades auxiliares
        public DateTime FechaCita { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreMedico { get; set; } = string.Empty;
    }
}