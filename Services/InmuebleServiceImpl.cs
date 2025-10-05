using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class InmuebleServiceImpl : InmuebleService
    {
        private readonly InmuebleRepository inmuebleRepository;

        public InmuebleServiceImpl(InmuebleRepository inmuebleRepository)
        {
            this.inmuebleRepository = inmuebleRepository;
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
    }
}