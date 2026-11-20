namespace MediCitas.Models
{
    public class EspecialidadResumen
    {
        public string Especialidad { get; set; } = string.Empty;
        public int CantidadDoctores { get; set; }
        public string DoctoresNombres { get; set; } = string.Empty;
    }
}