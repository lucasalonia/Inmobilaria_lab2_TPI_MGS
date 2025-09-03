using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InmuebleService
    {
        IList<Inmueble> ObtenerTodos();
        public IList<Inmueble> ObtenerTodosParaContratos(int paginaNro = 1, int tamPagina = 5);

        int Modificar(Inmueble inmueble);

        //int Baja(int idInmueble);

        int BajaLogica(int id, int? modificadoPor = null);

        Inmueble ObtenerPorId(int id);

        int Alta(Inmueble inmueble);
        int ObtenerCantidadInmueblesActivos();
    }
}