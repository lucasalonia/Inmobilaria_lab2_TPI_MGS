namespace Inmobilaria_lab2_TPI_MGS.Models.ViewModels
{
    public class InquilinoViewModel
    {
        public Persona Persona { get; set; } = new Persona();
        public Inquilino Inquilino { get; set; } = new Inquilino();
        public Direccion Direccion { get; set; } = new Direccion();
        public Contrato Contrato { get; set; } = new Contrato();
         
         public Inmueble Inmueble { get; set; } = new Inmueble();

    }
}