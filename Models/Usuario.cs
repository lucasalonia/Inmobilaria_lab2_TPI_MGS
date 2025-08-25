namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public Persona Persona { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? UltimoLogin { get; set; } 
        public Usuario? CreadoPor { get; set; }
        public Usuario? ModificadoPor { get; set; }
    }
}
