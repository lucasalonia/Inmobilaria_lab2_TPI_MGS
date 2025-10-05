using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La persona es requerida")]
        public Persona Persona { get; set; }
        
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string UserName { get; set; }
        
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? UltimoLogin { get; set; } 
        public Usuario? CreadoPor { get; set; }
        public Usuario? ModificadoPor { get; set; }
        
        public Rol? RolActual { get; set; }

        public string? FotoPerfil { get; set; }
    }
}
