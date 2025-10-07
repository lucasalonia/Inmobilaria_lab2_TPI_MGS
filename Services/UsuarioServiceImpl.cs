using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Models.ViewModels;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class UsuarioServiceImpl : UsuarioService
    {
        private readonly UsuarioRepository usuarioRepository;
        private readonly PersonService personService;
        private readonly UsuarioRolService usuarioRolService;

        public UsuarioServiceImpl(UsuarioRepository usuarioRepository, PersonService personService, UsuarioRolService usuarioRolService)
        {
            this.usuarioRepository = usuarioRepository;
            this.personService = personService;
            this.usuarioRolService = usuarioRolService;
        }
        
        public IList<Usuario> ObtenerTodos()
        {
            return usuarioRepository.ObtenerTodos();
        }

        public PaginatedListViewModel<Usuario> ObtenerTodosPaginados(int pageIndex = 1, int pageSize = 10)
        {
            var todosLosUsuarios = usuarioRepository.ObtenerTodos();
            return PaginatedListViewModel<Usuario>.Create(todosLosUsuarios, pageIndex, pageSize);
        }

        public Persona BuscarPersona(string dni)
        {
            return personService.ObtenerPorDni(dni);
        }

        public int CrearUsuario(Usuario usuario)
        {
            var personaExistente = personService.ObtenerPorDni(usuario.Persona.Dni);
            
            if (personaExistente == null)
            {
                usuario.Persona.FechaCreacion = DateTime.Now;
                usuario.Persona.FechaModificacion = DateTime.Now;
                int personaId = personService.CrearPersona(usuario.Persona);
                usuario.Persona.Id = personaId;
            }
            else
            {
                usuario.Persona = personaExistente;
            }

            usuario.FechaCreacion = DateTime.Now;
            usuario.FechaModificacion = DateTime.Now;
            return usuarioRepository.CrearUsuario(usuario);
        }

        public Usuario? ObtenerPorId(int id)
        {
            return usuarioRepository.ObtenerPorId(id);
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            if (usuario.Persona != null)
            {
                usuario.Persona.FechaModificacion = DateTime.Now;
                personService.ActualizarPersona(usuario.Persona);
            }

            usuario.FechaModificacion = DateTime.Now;
            return usuarioRepository.ActualizarUsuario(usuario);
        }

        public bool DeshabilitarUsuario(int id)
        {
            return usuarioRepository.DeshabilitarUsuario(id);
        }

        public IList<Rol> ObtenerRolesActivosPorUsuarioId(int usuarioId)
        {
            // Obtener el rol activo actual (siguiendo el mismo patrón que en AuthServiceImpl)
            var rolActivo = usuarioRolService.ObtenerActivoPorUsuarioId(usuarioId);
            if (rolActivo != null)
            {
                return new List<Rol> { rolActivo.Rol };
            }
            return new List<Rol>();
        }
    }
}