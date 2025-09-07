using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class UsuarioRolServiceImpl : UsuarioRolService
    {
        private readonly UsuarioRolRepository usuarioRolRepository;

        public UsuarioRolServiceImpl(UsuarioRolRepository usuarioRolRepository)
        {
            this.usuarioRolRepository = usuarioRolRepository;
        }

        public IList<UsuarioRol> ObtenerPorUsuarioId(int usuarioId)
        {
            return usuarioRolRepository.ObtenerPorUsuarioId(usuarioId);
        }

        public IList<UsuarioRol> ObtenerTodosPorUsuarioId(int usuarioId)
        {
            return usuarioRolRepository.ObtenerTodosPorUsuarioId(usuarioId);
        }

        public UsuarioRol? ObtenerActivoPorUsuarioId(int usuarioId)
        {
            return usuarioRolRepository.ObtenerActivoPorUsuarioId(usuarioId);
        }

        public int CrearUsuarioRol(UsuarioRol usuarioRol)
        {
            return usuarioRolRepository.CrearUsuarioRol(usuarioRol);
        }

        public bool ActualizarUsuarioRol(UsuarioRol usuarioRol)
        {
            return usuarioRolRepository.ActualizarUsuarioRol(usuarioRol);
        }

        public bool AsignarRolAUsuario(int usuarioId, int rolId, int? creadoPor = null)
        {
            DeshabilitarRolesUsuario(usuarioId);

            var usuarioRol = new UsuarioRol
            {
                Usuario = new Usuario { Id = usuarioId },
                Rol = new Rol { Id = rolId },
                Estado = "ACTIVO",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                CreadoPor = creadoPor.HasValue ? new Usuario { Id = creadoPor.Value } : null
            };

            var id = CrearUsuarioRol(usuarioRol);
            return id > 0;
        }

        public bool CambiarRolUsuario(int usuarioId, int nuevoRolId, int? modificadoPor = null)
        {
            var todosLosRoles = ObtenerTodosPorUsuarioId(usuarioId);
            var rolExistente = todosLosRoles.FirstOrDefault(ur => ur.Rol.Id == nuevoRolId);
            
            if (rolExistente != null)
            {
                // Si el usuario ya tuvo este rol, deshabilitar todos los roles activos
                // y reactivar el rol existente
                DeshabilitarRolesUsuario(usuarioId);
                return ReactivarRolUsuario(usuarioId, nuevoRolId, modificadoPor);
            }
            else
            {
                // Si es un rol nuevo, deshabilitar todos los roles activos
                // y crear uno nuevo
                DeshabilitarRolesUsuario(usuarioId);
                return AsignarRolAUsuario(usuarioId, nuevoRolId, modificadoPor);
            }
        }

        public bool DeshabilitarRolesUsuario(int usuarioId)
        {
            return usuarioRolRepository.DeshabilitarRolesUsuario(usuarioId);
        }

        public bool ReactivarRolUsuario(int usuarioId, int rolId, int? modificadoPor = null)
        {
            return usuarioRolRepository.ReactivarRolUsuario(usuarioId, rolId, modificadoPor);
        }
    }
}
