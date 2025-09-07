using Inmobilaria_lab2_TPI_MGS.Models;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public interface UsuarioRolService
    {
        IList<UsuarioRol> ObtenerPorUsuarioId(int usuarioId);
        IList<UsuarioRol> ObtenerTodosPorUsuarioId(int usuarioId);
        UsuarioRol? ObtenerActivoPorUsuarioId(int usuarioId);
        int CrearUsuarioRol(UsuarioRol usuarioRol);
        bool ActualizarUsuarioRol(UsuarioRol usuarioRol);
        bool AsignarRolAUsuario(int usuarioId, int rolId, int? creadoPor = null);
        bool CambiarRolUsuario(int usuarioId, int nuevoRolId, int? modificadoPor = null);
        bool DeshabilitarRolesUsuario(int usuarioId);
        bool ReactivarRolUsuario(int usuarioId, int rolId, int? modificadoPor = null);
    }
}
