using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class ContratoServiceImpl : ContratoService
    {
        private readonly ContratoRepository contratoRepository;

        public ContratoServiceImpl(ContratoRepository contratoRepository)
        {
            this.contratoRepository = contratoRepository;
        }
         public bool Alta(Contrato contrato)
        {
            return contratoRepository.Alta(contrato);
        }
        public bool Modificar(Contrato contrato)
        {
            return contratoRepository.Modificar(contrato);
        }
        public bool Baja(int id)
        {
            return contratoRepository.Baja(id);
        }
        public Contrato ObtenerContratoVigente(int inquilinoId)
        {
            return contratoRepository.ObtenerContratoVigente(inquilinoId);
        }
        public IList<Inmueble> ListarInmueblesDisponibles(int paginaNro = 1, int tamPagina = 10)
        {
            return contratoRepository.ListarInmueblesDisponibles();
        }
       
        public IList<Contrato> ListaContratosVigentes()
        {
            return contratoRepository.ListaContratosVigentes();
        }
    }
}
