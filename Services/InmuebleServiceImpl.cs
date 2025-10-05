using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class InmuebleServiceImpl : InmuebleService
    {
        private readonly InmuebleRepository inmuebleRepository;
        private readonly ContratoRepository contratoRepository;

        public InmuebleServiceImpl(InmuebleRepository inmuebleRepository, ContratoRepository contratoRepository)
        {
            this.inmuebleRepository = inmuebleRepository;
            this.contratoRepository = contratoRepository;
        }

        public int Alta(Inmueble inmueble)
        {
            return inmuebleRepository.Alta(inmueble);
        }
        public IList<Inmueble> ObtenerTodos()
        {
            return inmuebleRepository.ObtenerTodos();
        }
        
        //Sobre carga para usar en paginado de inmuebles - LS
        public IList<Inmueble> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
        {
            return inmuebleRepository.ObtenerTodos(paginaNro, tamPagina);
        }

        public int Modificar(Inmueble inmueble)
        {
            return inmuebleRepository.Modificar(inmueble);
        }

        public Inmueble ObtenerPorId(int id)
        {
            return inmuebleRepository.ObtenerPorId(id);
        }

        public int BajaLogica(int id, int? modificadoPor = null)
        {
            return inmuebleRepository.BajaLogica(id, modificadoPor);
        }
        public int ObtenerCantidadInmueblesActivos()
        {
            return inmuebleRepository.ObtenerCantidadInmueblesActivos();
        }
        public IList<Inmueble> ObtenerTodosParaContratos(int paginaNro = 1, int tamPagina = 5)
        {
            return inmuebleRepository.ObtenerTodosParaContratos(paginaNro, tamPagina);
        }
        public int ObtenerCantidadInmuebles()
        {
            return inmuebleRepository.ObtenerCantidadInmuebles();
        }

        public IList<Contrato> ObtenerContratosPorInmuebleId(int inmuebleId)
        {
            return contratoRepository.ObtenerPorInmuebleId(inmuebleId);
        }
        public Models.ViewModels.PaginatedListViewModel<Contrato> ObtenerContratosPorInmuebleId(int inmuebleId, int pagina, int tamPagina)
        {
            var total = contratoRepository.ContarPorInmuebleId(inmuebleId);
            var items = contratoRepository.ObtenerPorInmuebleId(inmuebleId, pagina, tamPagina);
            return Models.ViewModels.PaginatedListViewModel<Contrato>.Create(items, pagina, tamPagina);
        }
        public int ContarContratosPorInmuebleId(int inmuebleId)
        {
            return contratoRepository.ContarPorInmuebleId(inmuebleId);
        }
    }
}