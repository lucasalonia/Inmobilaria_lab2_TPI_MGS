namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Direccion
    {
        
        public int Id { get; set; }
        public Persona Persona { get; set; }
        public string? Calle { get; set; }
        public string? Numero { get; set; }
        public string? Piso { get; set; } // puede ser null
        public string? Departamento { get; set; } // puede ser null
        public string? Barrio { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public string Cp { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }
    }
}