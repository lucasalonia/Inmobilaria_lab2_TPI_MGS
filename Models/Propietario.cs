

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        public required Persona Persona { get; set; }
        public required string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }
        
   
    }
    
}