namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string Dni { get; set; }
        public string Sexo { get; set; } // 'M', 'F' o 'X'
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }  // puede ser null
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

    }
}