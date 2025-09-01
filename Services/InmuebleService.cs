using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface InmuebleService
    {
        IList<Inmueble> ObtenerTodos();

        //int Modificar(Inmueble inmueble);

        //int Baja(int idInmueble);

        //int BajaLogica(int id, int? modificadoPor = null);

        //Propietario ObtenerPorId(int propietarioId);

        int Alta(Inmueble inmueble);
    }
}