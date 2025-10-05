using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class ImagenServiceImpl : ImagenService
    {
        private readonly ImagenRepository repo;

        public ImagenServiceImpl(ImagenRepository repo)
        {
            this.repo = repo;
        } 

        public IList<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            return repo.BuscarPorInmueble(inmuebleId);
        }

        public void Alta(Imagen imagen)
        {
            repo.Alta(imagen);
        }
        public void Eliminar(int id)
        {
            repo.Eliminar(id);
        }
    }
}
