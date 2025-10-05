using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface ImagenService
    {
        IList<Imagen> BuscarPorInmueble(int inmuebleId);
        void Alta(Imagen imagen);
        void Eliminar(int id);
    }
}
