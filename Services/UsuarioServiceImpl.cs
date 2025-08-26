using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class UsuarioServiceImpl : UsuarioService
    {
        private readonly UsuarioRepository usuarioRepository;

        public UsuarioServiceImpl(UsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }

        public IList<Usuario> ObtenerTodos()
        {
            return usuarioRepository.ObtenerTodos();
        }
        
    }
}