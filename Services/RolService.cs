using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface RolService
    {
        IList<Rol> ObtenerTodos();
        Rol? ObtenerPorId(int id);
        Rol? ObtenerPorCodigo(string codigo);
    }
}
