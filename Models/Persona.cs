using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Persona
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI debe contener solo números")]
        public string Dni { get; set; }
        
        [Required(ErrorMessage = "El sexo es requerido")]
        public string Sexo { get; set; } // 'M', 'F' o 'X'
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public string Apellido { get; set; }
        
        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime? FechaNacimiento { get; set; }  // puede ser null
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(255, ErrorMessage = "El email no puede exceder los 255 caracteres")]
        public string? Email { get; set; }
        
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string? Telefono { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

    }
}