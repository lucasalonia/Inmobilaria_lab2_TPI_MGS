namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        public int PropietarioId { get; set; }
        public string Estado { get; set; }
        public string? Tipo { get; set; }
        public int? SuperficieM2 { get; set; }
        public int? Ambientes { get; set; }
        public int? Banos { get; set; }
        public int Cochera { get; set; }
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }
        
        public string? PropietarioNombre { get; set; }

    }
}