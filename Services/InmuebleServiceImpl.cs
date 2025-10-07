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

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            return inmuebleRepository.BuscarPorPropietario(idPropietario);
        }  
        public IList<Inmueble> BuscarDisponibles()
        {
            return inmuebleRepository.BuscarDisponibles();
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

        public int Modificar(Inmueble inmueble, int? modificadoPor = null)
        {
            return inmuebleRepository.Modificar(inmueble, modificadoPor);
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

        public IList<Inmueble> ObtenerDisponiblesEnRangoFechas(DateTime fechaInicio, DateTime fechaFin, int paginaNro = 1, int tamPagina = 5)
        {
            return contratoRepository.ListarInmueblesDisponiblesEnRangoFechas(fechaInicio, fechaFin, paginaNro, tamPagina);
        }
        public int ObtenerCantidadInmuebles()
        {
            return inmuebleRepository.ObtenerCantidadInmuebles();
        }
        public void ModificarPortada(int inmuebleId, string nuevaUrl)
        {
            inmuebleRepository.ModificarPortada(inmuebleId, nuevaUrl);
        }
        public void QuitarPortada(int inmuebleId)
        {
            inmuebleRepository.QuitarPortada(inmuebleId);
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