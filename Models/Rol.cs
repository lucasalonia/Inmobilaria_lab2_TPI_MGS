using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Rol
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(120, ErrorMessage = "El nombre del rol no puede exceder los 120 caracteres")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "El código del rol es requerido")]
        [StringLength(60, ErrorMessage = "El código del rol no puede exceder los 60 caracteres")]
        public string Codigo { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Usuario? CreadoPor { get; set; }
        public Usuario? ModificadoPor { get; set; }
    }
}
