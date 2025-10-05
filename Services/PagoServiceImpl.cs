using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class PagoServiceImpl : PagoService
    {
        private readonly PagoRepository pagoRepository;

        public PagoServiceImpl(PagoRepository pagoRepository)
        {
            this.pagoRepository = pagoRepository;
        }

        public bool Alta(Pago pago, int? idUsuario)
        {
            return pagoRepository.Alta(pago, idUsuario);
        }

        public bool Modificar(Pago pago, int? idUsuario)
        {
            return pagoRepository.Modificar(pago, idUsuario);
        }

        public bool Baja(int id, int? idUsuario)
        {
            return pagoRepository.Baja(id, idUsuario);
        }

        public Pago ObtenerPorId(ulong id)
        {
            return pagoRepository.ObtenerPorId(id);
        }

        public IList<Pago> ListarPorContrato(int contratoId)
        {
            return pagoRepository.ListarPorContrato(contratoId);
        }


    }
}
