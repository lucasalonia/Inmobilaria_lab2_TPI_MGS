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
        public bool Alta(Contrato contrato, int? idUsuario)
        {
            return contratoRepository.Alta(contrato, idUsuario);
        }
        public bool Modificar(Contrato contrato, int? idUsuario)
        {
            return contratoRepository.Modificar(contrato, idUsuario);
        }
        public bool Baja(int id, int? idUsuario)
        {
            return contratoRepository.Baja(id, idUsuario);
        }
        public Contrato ObtenerContratoVigente(int inquilinoId)
        {
            return contratoRepository.ObtenerContratoVigente(inquilinoId);
        }
        public IList<Inmueble> ListarInmueblesDisponibles(int paginaNro = 1, int tamPagina = 10)
        {
            return contratoRepository.ListarInmueblesDisponibles();
        }

        public IList<Contrato> ListaContratosVigentes(int paginaNro = 1, int tamPagina = 10)
        {
            return contratoRepository.ListaContratosVigentes(paginaNro, tamPagina);
        }
        public Contrato ObtenerPorId(int contratoId)
        {
            return contratoRepository.ObtenerPorId(contratoId);
        }
        public Contrato? ObtenerContratoVigentePorInmuebleId(int inmuebleId)
        { 
            return contratoRepository.ObtenerContratoVigentePorInmuebleId(inmuebleId);
        }
        public IList<Contrato> ObtenerPorInmuebleId(int inmuebleId)
        {
            return contratoRepository.ObtenerPorInmuebleId(inmuebleId);
        }

        public IList<Contrato> ObtenerTodos(int? inquilinoId = null)
        {
            return contratoRepository.ObtenerTodos(inquilinoId);
        }
    }
}
