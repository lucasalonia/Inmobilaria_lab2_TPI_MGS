using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface UsuarioService
    {
        IList<Usuario> ObtenerTodos();
        PaginatedListViewModel<Usuario> ObtenerTodosPaginados(int pageIndex = 1, int pageSize = 10);
        Persona BuscarPersona(string dni);
        int CrearUsuario(Usuario usuario);
        Usuario? ObtenerPorId(int id);
        bool ActualizarUsuario(Usuario usuario);
        bool DeshabilitarUsuario(int id);
        IList<Rol> ObtenerRolesActivosPorUsuarioId(int usuarioId);
    }
}