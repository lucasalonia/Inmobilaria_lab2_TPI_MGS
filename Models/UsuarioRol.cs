using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class UsuarioRol
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El usuario es requerido")]
        public Usuario Usuario { get; set; }
        
        [Required(ErrorMessage = "El rol es requerido")]
        public Rol Rol { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Usuario? CreadoPor { get; set; }
        public Usuario? ModificadoPor { get; set; }
    }
}
