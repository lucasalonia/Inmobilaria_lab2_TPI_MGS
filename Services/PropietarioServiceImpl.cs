using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class PropietarioServiceImpl : PropietarioService
    {
        private readonly PropietarioRepository propietarioRepository;

        public PropietarioServiceImpl(PropietarioRepository propietarioRepository)
        {
            this.propietarioRepository = propietarioRepository;
        }

        public int Alta(Propietario propietario)
        {
            return propietarioRepository.Alta(propietario);
        }

        public int Baja(int propietarioId)
        {
            return propietarioRepository.BajaLogica(propietarioId);
        }

        public int BajaLogica(int id, int? modificadoPor = null)
        {
            return propietarioRepository.BajaLogica(id, modificadoPor);
        }

        public int Modificar(Propietario propietario)
        {
            return propietarioRepository.Modificar(propietario);
        }

        public Propietario ObtenerPorId(int propietarioId)
        {
            return propietarioRepository.ObtenerPorId(propietarioId);
        }
        
         public Persona? ObtenerPorDni(string dni)
        {
            return propietarioRepository.ObtenerPorDni(dni);
        }
        public IList<Propietario> ObtenerTodos()
        {
            return propietarioRepository.ObtenerTodos();
        }
    }
}