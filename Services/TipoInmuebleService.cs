using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface TipoInmuebleService
    {
        IList<TipoInmueble> ObtenerTodos();
        TipoInmueble? ObtenerPorId(int id);
        int Alta(TipoInmueble tipo);
        int Modificar(TipoInmueble tipo);
        int Eliminar(int id);
    }
}