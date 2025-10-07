using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InmuebleService
    {
        IList<Inmueble> ObtenerTodos(int paginaNro = 1, int tamPagina = 10);
        public IList<Inmueble> ObtenerTodosParaContratos(int paginaNro = 1, int tamPagina = 5);
        IList<Inmueble> ObtenerDisponiblesEnRangoFechas(DateTime fechaInicio, DateTime fechaFin, int paginaNro = 1, int tamPagina = 5);

        int Modificar(Inmueble inmueble, int? modificadoPor = null);

        IList<Inmueble> BuscarDisponibles();

        int BajaLogica(int id, int? modificadoPor = null);

        Inmueble ObtenerPorId(int id);
        IList<Inmueble> BuscarPorPropietario(int propietarioId);

        int Alta(Inmueble inmueble);
        int ObtenerCantidadInmueblesActivos();

        int ObtenerCantidadInmuebles();

        void ModificarPortada(int inmuebleId, string nuevaUrl);

        void QuitarPortada(int inmuebleId);
        IList<Contrato> ObtenerContratosPorInmuebleId(int inmuebleId);
        Models.ViewModels.PaginatedListViewModel<Contrato> ObtenerContratosPorInmuebleId(int inmuebleId, int pagina, int tamPagina);
        int ContarContratosPorInmuebleId(int inmuebleId);
    }
}