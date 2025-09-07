using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class RolServiceImpl : RolService
    {
        private readonly RolRepository rolRepository;

        public RolServiceImpl(RolRepository rolRepository)
        {
            this.rolRepository = rolRepository;
        }

        public IList<Rol> ObtenerTodos()
        {
            return rolRepository.ObtenerTodos();
        }

        public Rol? ObtenerPorId(int id)
        {
            return rolRepository.ObtenerPorId(id);
        }

        public Rol? ObtenerPorCodigo(string codigo)
        {
            return rolRepository.ObtenerPorCodigo(codigo);
        }
    }
}
