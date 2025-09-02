using Inmobilaria_lab2_TPI_MGS.Models;
using Inmobilaria_lab2_TPI_MGS.Repository;

namespace Inmobilaria_lab2_TPI_MGS.Services
{
    public class PersonServiceImpl : PersonService
    {
        private readonly PersonaRepository personaRepository;

        public PersonServiceImpl(PersonaRepository personaRepository)
        {
            this.personaRepository = personaRepository;
        }
        
        public Persona ObtenerPorDni(string dni)
        {
            return personaRepository.ObtenerPorDni(dni);
        }
    }
}