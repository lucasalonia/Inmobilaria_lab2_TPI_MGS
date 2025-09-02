using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class UsuarioServiceImpl : UsuarioService
    {
        private readonly UsuarioRepository usuarioRepository;
        private readonly PersonService personService;

        public UsuarioServiceImpl(UsuarioRepository usuarioRepository, PersonService personService)
        {
            this.usuarioRepository = usuarioRepository;
            this.personService = personService;
        }
        
        public IList<Usuario> ObtenerTodos()
        {
            return usuarioRepository.ObtenerTodos();
        }

        public Persona BuscarPersona(string dni)
        {
            return personService.ObtenerPorDni(dni);
        }
    }
}