using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobilaria_lab2_TPI_MGS.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public string Url { get; set; } = string.Empty; // ruta del archivo o URL

        [NotMapped]
        public IFormFile? Archivo { get; set; } // para subir imágenes desde el form
        
        //FK hacia Inmueble
        //[ForeignKey("InmuebleId")]
        //public Inmueble Inmueble { get; set; }
    }
}