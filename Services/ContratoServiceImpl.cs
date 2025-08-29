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

        public Contrato ObtenerContratoVigente(int inquilinoId)
        {
            return contratoRepository.ObtenerContratoVigente(inquilinoId);
        }
    }
}
