using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class TipoInmuebleServiceImpl : TipoInmuebleService
    {
        private readonly TipoInmuebleRepository tipoinmuebleRepository;

        public TipoInmuebleServiceImpl(TipoInmuebleRepository tipoinmuebleRepository)
        {
            this.tipoinmuebleRepository = tipoinmuebleRepository;
        }

        public IList<TipoInmueble> ObtenerTodos()
        {
            return tipoinmuebleRepository.ObtenerTodos();
        }
        public TipoInmueble? ObtenerPorId(int id)
        {
            return tipoinmuebleRepository.ObtenerPorId(id);
        }
        public int Alta(TipoInmueble entidad)
        {
            return tipoinmuebleRepository.Alta(entidad);
        }
        public int Modificar(TipoInmueble entidad)
        {
            return tipoinmuebleRepository.Modificar(entidad);
        }
        public int Eliminar(int id)
        {
            return tipoinmuebleRepository.Eliminar(id);
        } 

    }
}