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

        public int Alta(Inmueble inmueble)
        {
            return inmuebleRepository.Alta(inmueble);
        }
        public IList<Inmueble> ObtenerTodos()
        {
            return inmuebleRepository.ObtenerTodos();
        }

        //Sobre carga para usar en paginado de inmuebles - LS
        public IList<Inmueble> ObtenerTodos(int paginaNro, int tamPagina)
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
    }
}