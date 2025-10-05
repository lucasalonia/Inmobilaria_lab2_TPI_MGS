using System.ComponentModel.DataAnnotations;

namespace Inmobilaria_lab2_TPI_MGS.Models.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe ingresar su contraseña actual para confirmar")]
        public string CurrentPassword { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
    }
}

