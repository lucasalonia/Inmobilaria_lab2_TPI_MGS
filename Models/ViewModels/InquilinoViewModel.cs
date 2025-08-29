namespace Inmobilaria_lab2_TPI_MGS.Models.ViewModels
{
    public class InquilinoCreateViewModel
    {
        public Persona Persona { get; set; } = new Persona();
        public Inquilino Inquilino { get; set; } = new Inquilino();
        public Direccion Direccion { get; set; } = new Direccion();
         public Contrato Contrato { get; set; } = new Contrato();

    }
}